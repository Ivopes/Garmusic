using Garmusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Repositories
{
    public interface ISongRepository
    {
        Task<IEnumerable<Song>> GetAllAsync(int accountID);
        Task<Song> GetByIdAsync(long id);
        Task PutAsync(Song song);
        Task MigrateSongs();
        Task SaveAsync();
    }
}
