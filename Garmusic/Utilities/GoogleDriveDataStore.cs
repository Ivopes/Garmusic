using Garmusic.Models;
using Garmusic.Models.Entities;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Utilities
{
    public class GoogleDriveDataStore : IDataStore
    {
        private readonly MusicPlayerContext _dbContext;
        private readonly StorageType _storageType = StorageType.GoogleDrive;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public GoogleDriveDataStore(MusicPlayerContext dbContext, IServiceScopeFactory serviceScopeFactory)
        {
            _dbContext = dbContext;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetAsync<T>(string key)
        {
            /*MusicPlayerContext _dbContext = this._dbContext;
            if (_dbContext is null)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                _dbContext = scope.ServiceProvider.GetRequiredService<MusicPlayerContext>();
            }*/

            int accountID = int.Parse(key);

            var entity = await _dbContext.AccountStorages.FindAsync(accountID, (int)_storageType);

            if (entity is null)
            {
                return default;
            }
            //string json = "{\"access_token\":\"ya29.A0AfH6SMByYeyL7xrNb1hKXgjakrJzCYcl33tDUBlJ97ov3_v - jeyvdJml9xDZUK1EJT5uKOD7uu_LPcihtKLfisWNlg8KqDaQfrxCyKoYWc0YRus55fn4AX4PrHRtk - YqPxaeNJpRTILmmepFb8RuoEUrSF9S\",\"token_type\":\"Bearer\",\"expires_in\":3599,\"refresh_token\":\"1//09wgaf2Ro-q3HCgYIARAAGAkSNwF-L9IrUxa-jNgsENfACF6vVajxFWxYuyoJ31g-m7bm1RdbhVYfAFXmTCwM1N5TTN3008ZoWm4\",\"scope\":\"https://www.googleapis.com/auth/drive.readonly\",\"id_token\":null,\"Issued\":\"2021-02-22T15:09:07.3025366+01:00\",\"IssuedUtc\":\"2021-02-22T14:09:07.3025366Z\"}";
            var token = JsonConvert.DeserializeObject<TokenResponse>(entity.JsonData);

            return (T)Convert.ChangeType(token, typeof(T));
        }

        public async Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key must have a value");
            }

            string json = JsonConvert.SerializeObject(value);

            AccountStorage accountStorage = new AccountStorage()
            {
                AccountID = int.Parse(key),
                StorageID = (int)_storageType,
                JsonData = json
            };

            await _dbContext.AccountStorages.AddAsync(accountStorage);

            await _dbContext.SaveChangesAsync();
        }
    }
}
