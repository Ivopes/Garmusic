using Garmusic.Interfaces.Repositories;
using Garmusic.Interfaces.Services;
using Garmusic.Models;
using Garmusic.Models.EntitiesWatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Services.Dependencies
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _playlistRepository;
        public PlaylistService(IPlaylistRepository playlistRepository)
        {
            _playlistRepository = playlistRepository;
        }
        public async Task<IEnumerable<Playlist>> GetAllAsync(int accountId)
        {
            return await _playlistRepository.GetAllAsync(accountId);
        }

        public async Task<IEnumerable<PlaylistWatch>> GetAllWatchAsync(int accountId)
        {
            return await _playlistRepository.GetAllWatchAsync(accountId);
        }

        public async Task<IEnumerable<Song>> GetSongsByIdAsync(int id)
        {
            return await _playlistRepository.GetSongsByIdAsync(id);
        }

        public async Task PostAsync(Playlist playlist)
        {
            await _playlistRepository.PostAsync(playlist);
        }

        public async Task UpdateSyncAsync(IEnumerable<Playlist> playlists)
        {
            await _playlistRepository.UpdateSyncAsync(playlists);
        }
    }
}
