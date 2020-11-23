using Garmusic.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Repositories
{
    public interface ISongRepository
    {
        Task<IEnumerable<Song>> GetAllAsync(int accountID);
        Task<Song> GetByIdAsync(int id);
        Task PostToDbxAsync(IFormFile file, int accountId);
        Task PostAsync(Song song);
        Task MigrateSongs(string token, string cursor);
        Task SaveAsync();
    }
}
