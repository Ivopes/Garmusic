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
        Task<Song> PostToDbxAsync(IFormFile file, int accountID);
        Task PostAsync(Song song);
        Task AddSongToPlaylistAsync(int sID, int plID);
        Task RemovePlaylistAsync(int sID, int plID);
        Task DeleteFromDbxAsync(int sID, int accountID);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountID">IDs of the users in storage system</param>
        /// <returns></returns>
        Task MigrateSongs(ICollection<string> accountIDs);
    }
}
