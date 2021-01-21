using Garmusic.Models;
using Garmusic.Models.EntitiesWatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Repositories
{
    public interface IPlaylistRepository
    {
        Task<IEnumerable<Playlist>> GetAllAsync(int accountId);
        Task<IEnumerable<PlaylistWatch>> GetAllWatchAsync(int accountId);
        Task PostAsync(Playlist playlist);
        Task<IEnumerable<Song>> GetSongsByIdAsync(int id);
        Task UpdateSyncAsync(IEnumerable<Playlist> playlists);
    }
}
