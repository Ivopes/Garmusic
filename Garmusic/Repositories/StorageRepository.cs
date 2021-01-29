using Garmusic.Interfaces.Repositories;
using Garmusic.Models;
using Garmusic.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Repositories
{
    public class StorageRepository : IStorageRepository
    {
        private readonly MusicPlayerContext _dbContext;
        public StorageRepository(MusicPlayerContext context)
        {
            _dbContext = context;
        }
        public async Task<IEnumerable<Storage>> GetAllAsync()
        {
            return await _dbContext.Storages.ToListAsync();
        }
    }
}
