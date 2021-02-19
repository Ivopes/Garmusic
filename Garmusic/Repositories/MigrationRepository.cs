using Dropbox.Api;
using Dropbox.Api.Files;
using Garmusic.Interfaces.Repositories;
using Garmusic.Interfaces.Utilities;
using Garmusic.Models;
using Garmusic.Models.Entities;
using Garmusic.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Garmusic.Repositories
{
    public class MigrationRepository : IMigrationRepository
    {
        private readonly MusicPlayerContext _dbContext;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly StorageType storageType = StorageType.Dropbox;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public MigrationRepository(MusicPlayerContext dbContext, IBackgroundTaskQueue backgroundTaskQueue, IServiceScopeFactory scopeFactory)
        {
            _dbContext = dbContext;
            _backgroundTaskQueue = backgroundTaskQueue;
            _serviceScopeFactory = scopeFactory;
        }
        public async Task DropboxWebhookMigrationAsync(IEnumerable<string> storageAccountsIDs)
        {

            var accounts = await _dbContext.Accounts.Include(a => a.AccountStorages.Where(acs => acs.StorageID == (int)StorageType.Dropbox)).ToListAsync();

            foreach (var acc in accounts)
            {
                if (acc.AccountStorages.Count == 0)
                {
                    continue;
                }

                DropboxJson json = JsonConvert.DeserializeObject<DropboxJson>(acc.AccountStorages[0].JsonData);

                if (json is null)
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

                //await UpdateSongs(acc.AccountID, files.Entries);

                await _dbContext.SaveChangesAsync();

                _backgroundTaskQueue.EnqueueAsync(ct => UpdateSongsMetadata(acc.AccountID, files.Entries, json.JwtToken));
            }
        }
        public async Task DropboxMigrationAsync(int accountId)
        {
            AccountStorage entity = await _dbContext.AccountStorages.FindAsync(accountId, (int)storageType);

            if (entity is null)
            {
                return;
            }

            DropboxJson dbxJson = JsonConvert.DeserializeObject<DropboxJson>(entity.JsonData);

            using var dbx = new DropboxClient(dbxJson.JwtToken);

            var files = await dbx.Files.ListFolderAsync("");

            dbxJson.Cursor = files.Cursor;

            await UpdateJsonData(accountId, dbxJson);

            //await UpdateSongs(accountId, files.Entries);

            await _dbContext.SaveChangesAsync();

            _backgroundTaskQueue.EnqueueAsync(ct => UpdateSongsMetadata(accountId, files.Entries, dbxJson.JwtToken));
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
                    var entity = await _dbContext.Songs.SingleOrDefaultAsync(s => s.FileName == song.Name && s.AccountID == accountId);
                    if (entity != null)
                    {
                        _dbContext.Songs.Remove(entity);
                    }
                }
                else
                {
                    // Check if alreaedy exists
                    var entity = await _dbContext.Songs.SingleOrDefaultAsync(s => s.FileName == song.Name && s.AccountID == accountId);

                    if (entity is null)
                    {
                        entity = new Song()
                        {
                            AccountID = accountId,
                            FileName = song.Name,
                            StorageID = (int)storageType,
                            StorageSongID = ((FileMetadata)song).Id
                        };

                        await _dbContext.Songs.AddAsync(entity);
                    }
                    else
                    {
                        entity.StorageSongID = ((FileMetadata)song).Id;
                    }
                }
            }
        }
        private async Task UpdateJsonData(int accountId, DropboxJson json)
        {
            var entity = await _dbContext.AccountStorages.FindAsync(accountId, (int)storageType);

            entity.JsonData = JsonConvert.SerializeObject(json);
        }
        private async Task UpdateSongsMetadata(int accountId, IEnumerable<Metadata> files, string jwtToken)
        {
            using var dbx = new DropboxClient(jwtToken);
            using var scope = _serviceScopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetRequiredService<MusicPlayerContext>();
            foreach (var song in files)
            {
                if (!song.Name.EndsWith(".mp3"))
                {
                    continue;
                }
                if (song.IsDeleted)
                {
                    var entity = await _dbContext.Songs.SingleOrDefaultAsync(s => s.FileName == song.Name && s.AccountID == accountId);
                    if (entity is not null)
                    {
                        _dbContext.Songs.Remove(entity);
                    }
                }
                else
                {
                    // Check if alreaedy exists
                    var entity = await _dbContext.Songs.SingleOrDefaultAsync(s => s.FileName == song.Name && s.AccountID == accountId);

                    if (entity is null)
                    {
                        entity = new Song()
                        {
                            AccountID = accountId,
                            FileName = song.Name,
                            StorageID = (int)storageType,
                            StorageSongID = ((FileMetadata)song).Id
                        };
                    }
                    var file = await dbx.Files.DownloadAsync(entity.StorageSongID);

                    var stream = await file.GetContentAsStreamAsync();
                    var mStream = new MemoryStream();
                    await stream.CopyToAsync(mStream);
                    MetadataUtility.FillMetadata(entity, mStream);

                    await _dbContext.Songs.AddAsync(entity);
                }
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
