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
            int accountID = int.Parse(key);

            var entity = await _dbContext.AccountStorages.FindAsync(accountID, (int)_storageType);

            if (entity is null)
            {
                return default;
            }
            
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
