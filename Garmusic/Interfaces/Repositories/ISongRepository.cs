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
        /// <summary>
        /// Get list of all Songs by account ID from storage
        /// </summary>
        /// <param name="accountID">ID of account to look for</param>
        /// <returns>List of songs</returns>
        Task<IEnumerable<Song>> GetAllAsync(int accountID);
        /// <summary>
        /// Get list of all Songs by account ID for watch from storage
        /// </summary>
        /// <param name="accountID">ID of account to look for</param>
        /// <returns>List of songs for watch</returns>
        Task<IEnumerable<SongWatch>> GetAllWatchAsync(int accountID);
        /// <summary>
        /// Get Songs by song ID from storage
        /// </summary>
        /// <param name="sID">song ID to look for</param>
        /// <returns>Song</returns>
        Task<Song> GetByIdAsync(int sID);
        /// <summary>
        /// Upload file to dbx to specified account
        /// </summary>
        /// <param name="file">File to upload</param>
        /// <param name="accountID">Accound ID to which upload the file</param>
        /// <returns>Song which was uploaded</returns>
        Task<Song> PostToDbxAsync(IFormFile file, int accountID);
        /// <summary>
        /// Post Song to database
        /// </summary>
        /// <param name="song">song to post</param>
        /// <returns></returns>
        Task PostAsync(Song song);
        /// <summary>
        /// Add Song to Playlist in storage
        /// </summary>
        /// <param name="sID">Song ID</param>
        /// <param name="plID">Playlist ID</param>
        /// <returns></returns>
        Task AddSongToPlaylistAsync(int sID, int plID);
        /// <summary>
        /// Removes Song from Playlist in storage
        /// </summary>
        /// <param name="sID">Song ID</param>
        /// <param name="plID">Playlist ID</param>
        /// <returns></returns>
        Task RemovePlaylistAsync(int sID, int plID);
        /// <summary>
        /// Deletes file from dbx
        /// </summary>
        /// <param name="sID">Song ID to delete</param>
        /// <param name="accountID">Account ID to which is Song linked</param>
        /// <returns></returns>
        Task DeleteFromDbxAsync(int sID, int accountID);
        /// <summary>
        /// Deletes files from dbx
        /// </summary>
        /// <param name="sIDs">Songs IDs to delete</param>
        /// <param name="accountID">Account ID to which is Song linked</param>
        /// <returns></returns>
        Task DeleteRangeFromDbxAsync(List<int> sIDs, int accountID);
        /// <summary>
        /// Get file from dbx
        /// </summary>
        /// <param name="sID">Song ID to get</param>
        /// <param name="accountID">Account ID to which is Song linked</param>
        /// <returns>File content</returns>
        Task<byte[]> GetDbxFileByIdAsync(int sID, int accountID);
        /// <summary>
        /// Is Song and Playlist linked with Account?
        /// </summary>
        /// <param name="accountID">Account ID to check</param>
        /// <param name="sID">Song ID to check</param>
        /// <param name="plID">Playlist ID to check</param>
        /// <returns>Can user modify?</returns>
        Task<bool> CanModifyAsync(int accountID, int sID, int plID);
        /// <summary>
        /// Is Song linked with Account?
        /// </summary>
        /// <param name="accountID">Account ID to check</param>
        /// <param name="sID">Song ID to check</param>
        /// <returns>Can user modify?</returns>
        Task<bool> CanModifyAsync(int accountID, int sID);
        /// <summary>
        /// Saves changes made in storage
        /// </summary>
        /// <returns></returns>
        Task SaveAsync();
    }
}
