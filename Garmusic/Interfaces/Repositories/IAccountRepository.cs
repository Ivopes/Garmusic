using Garmusic.Models;
using Garmusic.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<AccountWeb> GetByIdAsync(int id);
        Task PostAsync(Account account);
        Task SaveAsync();
    }
}
