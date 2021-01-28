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
        public async Task<AccountWeb> GetByIdAsync(int id)
        {
            var entity = await _dbContext.Accounts.Include(acc => acc.AccountStorages).ThenInclude(acs => acs.Storage).SingleOrDefaultAsync(acc => acc.AccountID == id);

            AccountWeb acc = null;

            if (entity is not null)
            {
                acc = new AccountWeb()
                {
                    Username = entity.Username,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Email = entity.Email,
                    Storage = entity.AccountStorages.Select(acs => acs.Storage).ToList()
                };
            }

            return acc;
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
