using Dropbox.Api;
using Garmusic.Interfaces.Repositories;
using Garmusic.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Repositories
{
    public class SongRepository : ISongRepository
    {
        private readonly string _dropConn = "sl.AlH_ZlL3NoPc6ox2uIyA_k2LFL9t7f8_CrTCRRZNshP7HGMslo1Re7TQB0d8W6CcNGEawSWKTyp2bjA5Yt56F1qHAHWBV530KUPPgLTnd64gwdwA0VMfHDoH_rmCLGvsGI6J_Vw";

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
        public async Task PostAsync(Song entity)
        {
            await _dbContext.Songs.AddAsync(entity);
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
