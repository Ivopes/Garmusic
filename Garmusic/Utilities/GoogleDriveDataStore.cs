using Garmusic.Models;
using Garmusic.Models.Entities;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Microsoft.EntityFrameworkCore;
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

        public GoogleDriveDataStore(MusicPlayerContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync<T>(string key)
        {
            int accountID = int.Parse(key);

            var entity = await _dbContext.AccountStorages.FindAsync(accountID, (int)_storageType);

            if (entity is null)
            {
                return;
            }

            _dbContext.AccountStorages.Remove(entity);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            int accountID = int.Parse(key);

            var entity = await _dbContext.AccountStorages.FindAsync(accountID, (int)_storageType);

            if (entity is null)
            {
                return default;
            }

            var json = JsonConvert.DeserializeObject<GoogleDriveJson>(entity.JsonData);

            var token = JsonConvert.DeserializeObject<TokenResponse>(json.Token);

            return (T)Convert.ChangeType(token, typeof(T));
        }

        public async Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key must have a value");
            }
            
            int accountID = int.Parse(key);

            var entity = await _dbContext.AccountStorages.FindAsync(accountID, (int)_storageType);

            if (entity is null)
            {
                string json = JsonConvert.SerializeObject(value);

                GoogleDriveJson gdJson = new GoogleDriveJson()
                {
                    Token = json
                };

                string jsonData = JsonConvert.SerializeObject(gdJson);

                AccountStorage accountStorage = new AccountStorage()
                {
                    AccountID = int.Parse(key),
                    StorageID = (int)_storageType,
                    JsonData = jsonData
                };

                await _dbContext.AccountStorages.AddAsync(accountStorage);
            }
            else
            {
                var json = JsonConvert.DeserializeObject<GoogleDriveJson>(entity.JsonData);

                json.Token = JsonConvert.SerializeObject(value);
                
                string jsonData = JsonConvert.SerializeObject(json);
                
                entity.JsonData = jsonData;
            }
            
            await _dbContext.SaveChangesAsync();
        }
    }
}
