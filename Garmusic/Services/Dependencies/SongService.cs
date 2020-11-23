using Garmusic.Interfaces.Repositories;
using Garmusic.Interfaces.Services;
using Garmusic.Models;
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
        public async Task PostToDbxAsync(IFormFile file, int accountId)
        {
            await _songRepo.PostToDbxAsync(file, accountId);
        }
        public async Task MigrateSongs(ICollection<string> accountIDs)
        {
            await _songRepo.MigrateSongs("", "");
        }

        public async Task PostAsync(Song song)
        {
            await _songRepo.PostAsync(song);
        }
    }
}
