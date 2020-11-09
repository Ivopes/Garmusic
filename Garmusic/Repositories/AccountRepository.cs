using Garmusic.Interfaces.Repositories;
using Garmusic.Models;
using Garmusic.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly MusicPlayerContext _dbContext;
        public AccountRepository(MusicPlayerContext context)
        {
            _dbContext = context;
        }
        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _dbContext.Accounts.Include(a => a.AccountStorages).ToListAsync();
        }
        public Task<Account> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task PostAsync(Account song)
        {
            throw new NotImplementedException();
        }
        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }
    }
}
