using Garmusic.Interfaces.Repositories;
using Garmusic.Interfaces.Services;
using Garmusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Services.Dependencies
{
    public class SongService : ISongService
    {
        private readonly ISongRepository _songRepo;
        private readonly IMigrationRepository _migRepo;
        public SongService(ISongRepository repository)
        {
            _songRepo = repository;
        }
        public async Task<IEnumerable<Song>> GetAllAsync(int accountID)
        {
            return await _songRepo.GetAllAsync(accountID);
        }
        public async Task<Song> GetByIdAsync(long id)
        {
            return await _songRepo.GetByIdAsync(id);
        }
        public async Task AddAsync(Song entity)
        {
            await _songRepo.PutAsync(entity);
        }
        public async Task MigrateSongs()
        {
            await _songRepo.MigrateSongs();
        }
    }
}
