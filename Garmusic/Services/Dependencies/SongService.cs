using Garmusic.Interfaces.Repositories;
using Garmusic.Interfaces.Services;
using Garmusic.Models;
using Garmusic.Models.EntitiesWatch;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Services.Dependencies
{
    public class SongService : ISongService
    {
        private readonly ISongRepository _songRepo;
        public SongService(ISongRepository repository)
        {
            _songRepo = repository;
        }
        public async Task<IEnumerable<Song>> GetAllAsync(int accountID)
        {
            return await _songRepo.GetAllAsync(accountID);
        }
        public async Task<Song> GetByIdAsync(int id)
        {
            return await _songRepo.GetByIdAsync(id);
        }
        public async Task<Song> PostToDbxAsync(IFormFile file, int accountId)
        {
            return await _songRepo.PostToDbxAsync(file, accountId);
        }
        public async Task PostAsync(Song song)
        {
            await _songRepo.PostAsync(song);
        }

        public async Task AddSongToPlaylistAsync(int sID, int plID)
        {
            await _songRepo.AddSongToPlaylistAsync(sID, plID);
        }

        public async Task RemovePlaylistAsync(int sID, int plID)
        {
            await _songRepo.RemovePlaylistAsync(sID, plID);
        }

        public async Task DeleteFromDbxAsync(int sID, int accountID)
        {
            await _songRepo.DeleteFromDbxAsync(sID, accountID);
        }

        public async Task<byte[]> GetDbxFileByIdAsync(int sID, int accountID)
        {
            return await _songRepo.GetDbxFileByIdAsync(sID, accountID);
        }

        public async Task<IEnumerable<SongWatch>> GetAllWatchAsync(int accountID)
        {
            return await _songRepo.GetAllWatchAsync(accountID);
        }
        public async Task<bool> CanModifyAsync(int accountID, int sID, int plID)
        {
            return await _songRepo.CanModifyAsync(accountID, sID, plID);
        }

        public async Task DeleteRangeFromDbxAsync(List<int> sIDs, int accountID)
        {
            await _songRepo.DeleteRangeFromDbxAsync(sIDs, accountID);
        }

        public async Task<bool> CanModifyAsync(int accountID, int sID)
        {
            return await _songRepo.CanModifyAsync(accountID, sID);
        }
    }
}
