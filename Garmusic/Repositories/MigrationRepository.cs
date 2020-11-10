using Dropbox.Api;
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
        public async Task DropboxMigrationAsync(IEnumerable<string> storageAccountsIDs)
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

                using var dbx = new DropboxClient(json.JwtToken);

                var files = await dbx.Files.ListFolderContinueAsync(json.Cursor);

                json.Cursor = files.Cursor;

                await UpdateJsonData(acc, json);
                await UpdateSongs(acc, files.Entries);

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task UpdateSongs(Account acc, IEnumerable<Metadata> files)
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
                        AccountID = acc.AccountID,
                        Name = song.Name,
                        StorageID = (int)storageType,
                        StorageSongID = ((FileMetadata)song).Id
                    };
                    await _dbContext.Songs.AddAsync(entity);
                }
            }
        }

        private async Task UpdateJsonData(Account account, DropboxJson json)
        {
            var entity = await _dbContext.AccountStorages.FindAsync(new object[] { account.AccountStorages[0].AccountID, account.AccountStorages[0].StorageID });

            entity.JsonData = JsonConvert.SerializeObject(json);
        }
    }
}
