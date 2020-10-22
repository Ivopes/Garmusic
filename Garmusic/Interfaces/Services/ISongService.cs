using Garmusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Services
{
    public interface ISongService
    {
        Task<IEnumerable<Song>> GetAllAsync(int accountID);
        Task<Song> GetByIdAsync(long id);
        Task AddAsync(Song song);
        Task MigrateSongs();
    }
}
