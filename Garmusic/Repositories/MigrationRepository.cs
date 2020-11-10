﻿using Dropbox.Api;
using Dropbox.Api.Files;
using Garmusic.Interfaces.Repositories;
using Garmusic.Models;
using Garmusic.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Garmusic.Repositories
{
    public class MigrationRepository : IMigrationRepository
    {
        private readonly MusicPlayerContext _dbContext;
        private readonly StorageType storageType = StorageType.Dropbox;
        public MigrationRepository(MusicPlayerContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task DropboxWebhookMigrationAsync(IEnumerable<string> storageAccountsIDs)
        {
            //TODO: update in EF core 5.0
            //var accounts = await _dbContext.Accounts.Include(a => a.AccountStorages.Where(acs => acs.StorageID == (int)StorageType.Dropbox )).ToListAsync();
            var accounts = await _dbContext.Accounts.Include(a => a.AccountStorages).AsNoTracking().ToListAsync();
            foreach (var acc in accounts)
            {
                acc.AccountStorages = acc.AccountStorages.Where(acs => acs.StorageID == (int)storageType).ToArray();
                
                if (acc.AccountStorages.Count != 1)
                {
                    continue;
                }

                DropboxJson json = JsonConvert.DeserializeObject<DropboxJson>(acc.AccountStorages[0].JsonData);

                if (json == null)
                {
                    continue;
                }

                if (!storageAccountsIDs.Contains(json.DropboxID))
                {
                    continue;
                }

                using var dbx = new DropboxClient(json.JwtToken);

                var files = await dbx.Files.ListFolderContinueAsync(json.Cursor);

                json.Cursor = files.Cursor;

                await UpdateJsonData(acc.AccountID, json);
                await UpdateSongs(acc.AccountID, files.Entries);

                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task DropboxMigrationAsync(int accountId)
        {
            AccountStorage entity = await _dbContext.AccountStorages.FindAsync(new object[] { accountId, (int)storageType });

            if (entity == null)
            {
                return;
            }

            DropboxJson dbxJson = JsonConvert.DeserializeObject<DropboxJson>(entity.JsonData);

            using var dbx = new DropboxClient(dbxJson.JwtToken);

            var files = await dbx.Files.ListFolderAsync("");

            dbxJson.Cursor = files.Cursor;

            await UpdateJsonData(accountId, dbxJson);
            await UpdateSongs(accountId, files.Entries);

            await _dbContext.SaveChangesAsync();
        }
        private async Task UpdateSongs(int accountId, IEnumerable<Metadata> files)
        {
            foreach (var song in files)
            {
                if (!song.Name.EndsWith(".mp3"))
                {
                    continue;
                }
                if (song.IsDeleted)
                {
                    var entity = await _dbContext.Songs.SingleOrDefaultAsync(s => s.Name == song.Name);
                    if(entity != null)
                    {
                        _dbContext.Songs.Remove(entity);
                    }
                }
                else
                {
                    //var entity = await _dbContext.Songs.SingleOrDefaultAsync(s => s.Name == song.Name);
                    //TODO dobry cast?
                    Song entity = new Song()
                    {
                        AccountID = accountId,
                        Name = song.Name,
                        StorageID = (int)storageType,
                        StorageSongID = ((FileMetadata)song).Id
                    };
                    await _dbContext.Songs.AddAsync(entity);
                }
            }
        }

        private async Task UpdateJsonData(int accountId, DropboxJson json)
        {
            var entity = await _dbContext.AccountStorages.FindAsync(new object[] { accountId, (int)storageType });

            entity.JsonData = JsonConvert.SerializeObject(json);
        }
    }
}
