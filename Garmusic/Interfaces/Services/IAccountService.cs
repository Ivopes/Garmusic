using Garmusic.Models;
using Garmusic.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<AccountWeb> GetByIdAsync(int id);
        Task AddAsync(Account account);
    }
}
