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

        public async Task<bool> CanModifyAsync(int accountID, int pID)
        {
            var pl = await _dbContext.Playlists.FindAsync(pID);

            if (pl is null)
            {
                return false;
            }

            return pl.AccountID == accountID;
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
                    SongsIds = songsWatchIds,
                    Sync = pl.Sync
                });
            }
            return playlistsWatch;
        }
        public async Task<IEnumerable<Song>> GetSongsByPlIdAsync(int id)
        {
            return await _dbContext.Playlists.Where(pl => pl.Id == id).Include(pl => pl.Songs).Select(pl => pl.Songs).SingleOrDefaultAsync();
        }

        public async Task PostAsync(Playlist playlist)
        {
            await _dbContext.Playlists.AddAsync(playlist);

            await SaveAsync();
        }

        public async Task RemoveAsync(int pID)
        {
            var entity = await _dbContext.Playlists.FindAsync(pID);

            if (entity is null)
            {
                return;
            }

            _dbContext.Playlists.Remove(entity);

            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateSyncAsync(IEnumerable<Playlist> playlists)
        {
            foreach (var pl in playlists)
            {
                var entity = _dbContext.Playlists.Find(pl.Id);

                entity.Sync = pl.Sync;
            }

            await SaveAsync();
        }
    }
}
