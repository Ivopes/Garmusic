using Garmusic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Services
{
    public interface IPlaylistService
    {
        Task<IEnumerable<Playlist>> GetAllAsync(int accountId);
        Task PostAsync(Playlist playlist);
    }
}