using Garmusic.Interfaces.Repositories;
using Garmusic.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Account>> GetAllByStorageAccountIDAsync(IEnumerable<string> storageAccountsIDs)
        {
            //var accounts = _dbContext.Accounts.Include(a => a.AccountStorages).AsEnumerable().Where(a => a.AccountStorages.Count != 0 && storageAccountsIDs.Contains(a.AccountStorages[0].StorageAccountID)).ToList();
            var accounts = await _dbContext.Accounts.Include(a => a.AccountStorages).ToListAsync();

            var a = new List<Account>();

            foreach (var acc in accounts)
            {
                if(acc.AccountStorages.Count == 0)
                {
                    continue;
                }
                if(storageAccountsIDs.Contains(acc.AccountStorages[0].StorageAccountID))
                {
                    a.Add(acc);
                }
            }
            return a;
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
