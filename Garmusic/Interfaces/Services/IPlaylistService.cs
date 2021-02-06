using Garmusic.Models;
using Garmusic.Models.EntitiesWatch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Services
{
    public interface IPlaylistService
    {
        /// <summary>
        /// Get all playlists by Account ID
        /// </summary>
        /// <param name="accountID">Accound ID from which take playlists</param>
        /// <returns>List of Playlists</returns>
        Task<IEnumerable<Playlist>> GetAllAsync(int accountID);
        /// <summary>
        /// Get list of Playlists for watch by Accound ID
        /// </summary>
        /// <param name="accountID">Accound ID from which take playlists</param>
        /// <returns>List of playlists for watch</returns>
        Task<IEnumerable<PlaylistWatch>> GetAllWatchAsync(int accountID);
        /// <summary>
        /// Post Playlist to storage
        /// </summary>
        /// <param name="playlist">Playlist to post</param>
        /// <returns></returns>
        Task PostAsync(Playlist playlist);
        /// <summary>
        /// Get list of Songs by Playlist ID from storage
        /// </summary>
        /// <param name="pID">Playlist ID from which take songs</param>
        /// <returns>List of Songs</returns>
        Task<IEnumerable<Song>> GetSongsByPlIdAsync(int pID);
        /// <summary>
        /// Update information if Playlist should be synced to watch
        /// </summary>
        /// <param name="playlists">playlist to update</param>
        /// <returns></returns>
        Task UpdateSyncAsync(IEnumerable<Playlist> playlists);
        /// <summary>
        /// Is Playlist linked with Account?
        /// </summary>
        /// <param name="accountID">Account ID to check</param>
        /// <param name="pID">Song ID to check</param>
        /// <returns>Can user modify?</returns>
        Task<bool> CanModifyAsync(int accountID, int pID);
        /// <summary>
        /// Deletes Playlist from storage
        /// </summary>
        /// <param name="pID">Playlists id</param>
        /// <returns></returns>
        Task RemoveAsync(int pID);
    }
}