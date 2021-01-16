using Garmusic.Models;
using Garmusic.Models.EntitiesWatch;
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
        Task<IEnumerable<SongWatch>> GetAllWatchAsync(int accountID);
        Task<Song> GetByIdAsync(int id);
        Task<Song> PostToDbxAsync(IFormFile file, int accountId);
        Task PostAsync(Song song);
        Task MigrateSongs(string token, string cursor);
        Task AddSongToPlaylistAsync(int sID, int plID);
        Task RemovePlaylistAsync(int sID, int plID);
        Task DeleteFromDbxAsync(int sID, int accountID);
        Task<byte[]> GetFileByIdAsync(int sID, int accountID);
        Task SaveAsync();
    }
}
