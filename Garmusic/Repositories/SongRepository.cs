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
        private readonly string _dropConn = "sl.AkHvoj0kVHbfNQbk0gNGyi5qH7rrEybyZBMfebwHYYm3DU-g2O6w435wh0JuQXNzrrr_RmMbKgfBE65k_3D9kv6GnQyppA4Nird9J9C6eiH-l5bSHGahHfDD1D3BhOgeYU9EasA";

        private readonly MusicPlayerContext _dbContext;
        public SongRepository(MusicPlayerContext context)
        {
            _dbContext = context;
        }
        public async Task<IEnumerable<Song>> GetAllAsync(int accountID)
        {
            return await _dbContext.Songs.Where(s => s.AccountID == accountID).ToListAsync();
        }
        public async Task<Song> GetByIdAsync(long id)
        {
            return await _dbContext.Songs.FindAsync(id);
        }
        public async Task MigrateSongs()
        {
            using var dbx = new DropboxClient(_dropConn);

            var files = await dbx.Files.ListFolderAsync(string.Empty);

            List<string> names = new List<string>();

            foreach (var file in files.Entries)
            {
                names.Add(file.Name);
            }
            //return names;
            //_dbContext.Songs
            foreach (var item in names)
            {
                await PutAsync(new Song{
                    AccountID = 1,
                    Name = item
                });
            }
            await SaveAsync();
        }
        public async Task PutAsync(Song entity)
        {
            await _dbContext.Songs.AddAsync(entity);
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
