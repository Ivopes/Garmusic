using Garmusic.Models;
using Garmusic.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Repositories
{
    public interface IAuthRepository
    {
        Task<string> LoginAsync(Account account);
        Task<string> RegisterAsync(Account account);
        Task RegisterDropboxAsync(int accountId, DropboxJson json);
    }
}
