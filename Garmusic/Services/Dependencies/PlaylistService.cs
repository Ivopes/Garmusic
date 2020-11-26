using Garmusic.Interfaces.Repositories;
using Garmusic.Interfaces.Services;
using Garmusic.Models;
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

        public async Task<IEnumerable<Song>> GetSongsById(int id)
        {
            return await _playlistRepository.GetSongsById(id);
        }

        public async Task PostAsync(Playlist playlist)
        {
            await _playlistRepository.PostAsync(playlist);
        }
    }
}
