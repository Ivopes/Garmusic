using Garmusic.Models;
using Garmusic.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Repositories
{
    public interface IAuthRepository
    {
        /// <summary>
        /// Check Account credentials and return JWT token if correct
        /// </summary>
        /// <param name="account">Account with credential to check</param>
        /// <returns>JWT token as string, otherwise empty string</returns>
        Task<string> LoginAsync(Account account);
        Task ChangePassword(int accountID, string oldPass, string newPass);

        /// <summary>
        /// Check Account credentials and return JWT token if correct with extend lifetime fo watch
        /// </summary>
        /// <param name="account">Account with credential to check</param>
        /// <returns>JWT token as string, otherwise empty string</returns>
        Task<string> LoginWatchAsync(Account account);
        /// <summary>
        /// Add Account to storage and return error message if wrong registration data
        /// </summary>
        /// <param name="account">Account to add</param>
        /// <returns>String with error message, otherwise empty string</returns>
        Task<string> RegisterAsync(Account account);
        /// <summary>
        /// Add AccountStorage entity to storage
        /// </summary>
        /// <param name="accountID">Account ID to add</param>
        /// <param name="json">data to add</param>
        /// <returns></returns>
        Task RegisterDropboxAsync(int accountID, DropboxJson json);
        /// <summary>
        /// Add AccountStorage entity to storage
        /// </summary>
        /// <param name="accountID">Account ID to add</param>
        /// <param name="token">JWT token to add</param>
        /// <returns></returns>
        Task RegisterGoogleDriveAsync(int accountID, string token);
        Task SignOutGoogleDrive(int accountId);

        /// <summary>
        /// Get JWT token by Account ID from storage
        /// </summary>
        /// <param name="accountID">Account ID from which take token</param>
        /// <returns>JWT token as string if exist, otherwise empty string</returns>
        Task<string> GetDropboxJwtAsync(int accountID);
        /// <summary>
        /// Get the hashed dropbox key and secret for dbxOAuth token url
        /// </summary>
        /// <returns>Key:secret in base64</returns>
        string GetDropboxKeys();
        Task SignOutDbx(int aID);
        Task SaveAsync();
    }
}
