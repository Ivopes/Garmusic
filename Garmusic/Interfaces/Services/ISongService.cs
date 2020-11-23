using Garmusic.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Services
{
    public interface ISongService
    {
        Task<IEnumerable<Song>> GetAllAsync(int accountID);
        Task<Song> GetByIdAsync(int id);
        Task PostToDbxAsync(IFormFile file, int accountId);
        Task PostAsync(Song song);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountID">IDs of the users in storage system</param>
        /// <returns></returns>
        Task MigrateSongs(ICollection<string> accountIDs);
    }
}
