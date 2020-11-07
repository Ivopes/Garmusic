using Garmusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<IEnumerable<Account>> GetAllByStorageAccountIDAsync(IEnumerable<string> storageAccountsIDs);
        Task<Account> GetByIdAsync(int id);
        Task PostAsync(Account account);
        Task SaveAsync();
    }
}
