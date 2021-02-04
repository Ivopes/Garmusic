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
        /// <summary>
        /// Get all Accounts
        /// </summary>
        /// <returns>List of Sccounts</returns>
        Task<IEnumerable<Account>> GetAllAsync();
        /// <summary>
        /// Get Account by ID for web
        /// </summary>
        /// <param name="accountID">Account ID of account to get</param>
        /// <returns>Account entity</returns>
        Task<AccountWeb> GetByIdAsync(int accountID);
        /// <summary>
        /// Post Account to storage
        /// </summary>
        /// <param name="account">Account to post</param>
        /// <returns></returns>
        Task PostAsync(Account account);
        /// <summary>
        /// Saves changes made in storage
        /// </summary>
        /// <returns></returns>
        Task SaveAsync();
    }
}
