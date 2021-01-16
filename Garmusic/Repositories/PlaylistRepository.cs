using Garmusic.Interfaces.Repositories;
using Garmusic.Models;
using Garmusic.Models.EntitiesWatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly MusicPlayerContext _dbContext;
        public PlaylistRepository(MusicPlayerContext context)
        {
            _dbContext = context;
        }
        public async Task<IEnumerable<Playlist>> GetAllAsync(int accountId)
        {
            return await _dbContext.Playlists.Where(pl => pl.AccountID == accountId).Include(pl => pl.Songs).ToListAsync();
        }

        public async Task<IEnumerable<PlaylistWatch>> GetAllWatchAsync(int accountId)
        {
            var playlists = await GetAllAsync(accountId);
            List<PlaylistWatch> playlistsWatch = new List<PlaylistWatch>();
            foreach (var pl in playlists)
            {
                var songsWatchIds = pl.Songs.Select(s => s.Id).ToArray();

                playlistsWatch.Add(new PlaylistWatch()
                {
                    Id = pl.Id,
                    Name = pl.Name,
                    SongsIds = songsWatchIds
                });
            }
            return playlistsWatch;
        }

        public async Task<IEnumerable<Song>> GetSongsById(int id)
        {
            return await _dbContext.Playlists.Where(pl => pl.Id == id).Include(pl => pl.Songs).Select(pl => pl.Songs).SingleOrDefaultAsync();
        }

        public async Task PostAsync(Playlist playlist)
        {
            await _dbContext.Playlists.AddAsync(playlist);

            await SaveAsync();
        }
        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
