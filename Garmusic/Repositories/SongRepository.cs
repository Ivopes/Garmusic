using Dropbox.Api;
using Dropbox.Api.Files;
using Garmusic.Interfaces.Repositories;
using Garmusic.Models;
using Garmusic.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Repositories
{
    public class SongRepository : ISongRepository
    {
        private readonly MusicPlayerContext _dbContext;
        public SongRepository(MusicPlayerContext context)
        {
            _dbContext = context;
        }
        public async Task<IEnumerable<Song>> GetAllAsync(int accountID)
        {
            return await _dbContext.Songs.Where(s => s.AccountID == accountID).ToListAsync();
        }
        public async Task<Song> GetByIdAsync(int id)
        {
            return await _dbContext.Songs.FindAsync(id);
        }
        public async Task MigrateSongs(string token, string cursor)
        {
            using var dbx = new DropboxClient(token);

            //var files = await dbx.Files.ListFolderAsync(string.Empty);
            
            var newFiles = await dbx.Files.ListFolderContinueAsync(cursor);
            List<string> names = new List<string>();

            foreach (var file in newFiles.Entries)
            {
                names.Add(file.Name);
            }

            foreach (var item in names)
            {
                await PostAsync(new Song{
                    AccountID = 1,
                    Name = item
                });
            }
            await SaveAsync();
        }

        public async Task PostAsync(Song song)
        {
            await _dbContext.Songs.AddAsync(song);
        }

        public async Task PostToDbxAsync(IFormFile file, int accountId)
        {
            var accountStorage = _dbContext.AccountStorages.Find(new object[] { accountId, (int)StorageType.Dropbox });
            
            DropboxJson dbxJson = JsonConvert.DeserializeObject<DropboxJson>(accountStorage.JsonData);

            using var dbx = new DropboxClient(dbxJson.JwtToken);
            
            var uploaded = await dbx.Files.UploadAsync(
                                                    "/" + file.FileName,
                                                    WriteMode.Add.Instance,
                                                    strictConflict: true,
                                                    body: file.OpenReadStream());
            Song song = new Song()
            {
                AccountID = accountId,
                Name = file.FileName,
                StorageSongID = uploaded.Id,
                StorageID = (int)StorageType.Dropbox
            };

            await PostAsync(song);

            await SaveAsync();
        }
        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
