using Garmusic.Interfaces.Repositories;
using Garmusic.Interfaces.Services;
using Garmusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Garmusic.Services.Dependencies
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accRepo;
        public AccountService(IAccountRepository accountRepository)
        {
            _accRepo = accountRepository;
        }
        public Task AddAsync(Account account)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _accRepo.GetAllAsync();
        }

        public async Task<IEnumerable<Account>> GetAllByStorageAccountIDAsync(IEnumerable<string> storageAccountsIDs)
        {
            return await _accRepo.GetAllByStorageAccountIDAsync(storageAccountsIDs);
        }

        public Task<Account> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
