using Garmusic.Interfaces.Repositories;
using Garmusic.Interfaces.Services;
using Garmusic.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Services.Dependencies
{
    public class StorageService : IStorageService
    {
        private readonly IStorageRepository _storageRepository;
        public StorageService(IStorageRepository storageRepository)
        {
            _storageRepository = storageRepository;
        }
        public async Task<IEnumerable<Storage>> GetAllAsync()
        {
            return await _storageRepository.GetAllAsync();
        }
    }
}
