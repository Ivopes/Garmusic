using Dropbox.Api;
using Dropbox.Api.Files;
using Garmusic.Interfaces.Repositories;
using Garmusic.Interfaces.Utilities;
using Garmusic.Models;
using Garmusic.Models.Entities;
using Garmusic.Utilities;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Garmusic.Repositories
{
    public class MigrationRepository : IMigrationRepository
    {
        private readonly MusicPlayerContext _dbContext;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IDataStore _GDdataStore;
        private readonly IWebHostEnvironment _env;

        private readonly string[] _gdScopes = { DriveService.Scope.Drive };

        public MigrationRepository(MusicPlayerContext dbContext, IBackgroundTaskQueue backgroundTaskQueue, IServiceScopeFactory scopeFactory, IDataStore gDdataStore, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _backgroundTaskQueue = backgroundTaskQueue;
            _serviceScopeFactory = scopeFactory;
            _GDdataStore = gDdataStore;
            _env = env;
        }
        public async Task DropboxWebhookMigrationAsync(IEnumerable<string> storageAccountsIDs)
        {

            var accounts = await _dbContext.Accounts.Include(a => a.AccountStorages.Where(acs => acs.StorageID == (int)StorageType.Dropbox)).ToListAsync();

            foreach (var acc in accounts)
            {
                if (acc.AccountStorages.Count == 0)
                {
                    continue;
                }

                DropboxJson json = JsonConvert.DeserializeObject<DropboxJson>(acc.AccountStorages[0].JsonData);

                if (json is null)
                {
                    continue;
                }

                if (!storageAccountsIDs.Contains(json.DropboxID))
                {
                    continue;
                }

                using var dbx = new DropboxClient(json.JwtToken);

                var files = await dbx.Files.ListFolderContinueAsync(json.Cursor);

                json.Cursor = files.Cursor;

                await UpdateJsonData(acc.AccountID, json);

                //await UpdateSongs(acc.AccountID, files.Entries);

                await _dbContext.SaveChangesAsync();

                if (_env.IsDevelopment())
                {
                    _backgroundTaskQueue.EnqueueAsync(ct => UpdateSongsDBX(acc.AccountID, files.Entries, json.JwtToken, true));
                }
                else
                {
                    await UpdateSongsDBX(acc.AccountID, files.Entries, json.JwtToken);
                }
            }
        }
        public async Task DropboxMigrationAsync(int accountId)
        {
            AccountStorage entity = await _dbContext.AccountStorages.FindAsync(accountId, (int)StorageType.Dropbox);

            if (entity is null)
            {
                return;
            }

            DropboxJson dbxJson = JsonConvert.DeserializeObject<DropboxJson>(entity.JsonData);

            using var dbx = new DropboxClient(dbxJson.JwtToken);

            var files = await dbx.Files.ListFolderAsync(string.Empty);

            dbxJson.Cursor = files.Cursor;

            await UpdateJsonData(accountId, dbxJson);

            //await UpdateSongs(accountId, files.Entries);

            await _dbContext.SaveChangesAsync();

            if (_env.IsDevelopment())
            {
                _backgroundTaskQueue.EnqueueAsync(ct => UpdateSongsDBX(accountId, files.Entries, dbxJson.JwtToken));
            }
            else
            {
                await UpdateSongsDBX(accountId, files.Entries, dbxJson.JwtToken);
            }
        }
        public async Task GoogleDriveMigrationAsync(int accountId)
        {
            AccountStorage entity = await _dbContext.AccountStorages.FindAsync(accountId, (int)StorageType.GoogleDrive);

            if (entity is null)
            {
                return;
            }

            using var stream = new FileStream("googleDriveSecrets.json", FileMode.Open, FileAccess.Read);
            
            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                _gdScopes,
                accountId.ToString(),
                CancellationToken.None,
                _GDdataStore);
            
            using var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Garmusic",
            });

            FilesResource.ListRequest listRequest = service.Files.List();

            listRequest.Q = "mimeType='audio/mpeg'";
            
            var files = await listRequest.ExecuteAsync();
            
            if (_env.IsDevelopment())
            {
                _backgroundTaskQueue.EnqueueAsync(ct => UpdateSongsGD(accountId, files.Files, true));
            }
            else
            {
                await UpdateSongsGD(accountId, files.Files);
            }

            var token = await service.Changes.GetStartPageToken().ExecuteAsync();
            var channel = new Channel
            {
                Type = "web_hook",
                Id = Guid.NewGuid().ToString(),
                Address = "https://garmusic.azurewebsites.net/api/webhook/googledrive"
            };

            await service.Changes.Watch(channel, token.StartPageTokenValue).ExecuteAsync();

            var gdData = JsonConvert.DeserializeObject<GoogleDriveJson>(entity.JsonData);

            gdData.ChannelId = channel.Id;

            gdData.StartPageToken = token.StartPageTokenValue;

            entity.JsonData = JsonConvert.SerializeObject(gdData);

            await _dbContext.SaveChangesAsync();
        }
        private async Task UpdateJsonData(int accountId, DropboxJson json)
        {
            var entity = await _dbContext.AccountStorages.FindAsync(accountId, (int)StorageType.Dropbox);

            entity.JsonData = JsonConvert.SerializeObject(json);
        }
        private async Task UpdateSongsDBX(int accountId, IEnumerable<Metadata> files, string jwtToken, bool updateMetadata = false)
        {
            using var dbx = new DropboxClient(jwtToken);
            using var scope = _serviceScopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetRequiredService<MusicPlayerContext>();
            foreach (var song in files)
            {
                if (!song.Name.EndsWith(".mp3"))
                {
                    continue;
                }
                if (song.IsDeleted)
                {
                    var entity = await _dbContext.Songs.SingleOrDefaultAsync(s => s.FileName == song.Name && s.AccountID == accountId && s.StorageID == (int)StorageType.Dropbox);
                    if (entity is not null)
                    {
                        _dbContext.Songs.Remove(entity);
                    }
                }
                else
                {
                    // Check if alreaedy exists
                    var entity = await _dbContext.Songs.SingleOrDefaultAsync(
                        s => s.FileName == song.Name && 
                        s.AccountID == accountId && 
                        s.StorageID == (int)StorageType.Dropbox
                        );

                    bool alreadyIn = true;

                    if (entity is null)
                    {
                        entity = new Song()
                        {
                            AccountID = accountId,
                            Name = song.Name[..song.Name.LastIndexOf('.')],
                            FileName = song.Name,
                            StorageID = (int)StorageType.Dropbox,
                            StorageSongID = ((FileMetadata)song).Id
                        };
                        alreadyIn = false;
                    }

                    if (updateMetadata)
                    {
                        var file = await dbx.Files.DownloadAsync(entity.StorageSongID);
                        var stream = await file.GetContentAsStreamAsync();
                        var mStream = new MemoryStream();
                        await stream.CopyToAsync(mStream);
                        MetadataUtility.FillMetadata(entity, mStream);
                    }

                    if (!alreadyIn)
                    {
                        await _dbContext.Songs.AddAsync(entity);
                    }
                }
            }
            await _dbContext.SaveChangesAsync();
        }
        private async Task UpdateSongsGD(int accountId, IList<Google.Apis.Drive.v3.Data.File> files, bool updateMetadata = false)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var gdDataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();

            var _dbContext = scope.ServiceProvider.GetRequiredService<MusicPlayerContext>();
            
            using var stream = new FileStream("googleDriveSecrets.json", FileMode.Open, FileAccess.Read);

            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                _gdScopes,
                accountId.ToString(),
                CancellationToken.None,
                gdDataStore);

            using var gdService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Garmusic",
            });

            foreach (var song in files)
            {
                if (!song.Name.EndsWith(".mp3"))
                {
                    continue;
                }

                // Check if alreaedy exists
                var entity = await _dbContext.Songs.SingleOrDefaultAsync(s => 
                    s.FileName == song.Name && 
                    s.AccountID == accountId && 
                    s.StorageID == (int)StorageType.GoogleDrive);

                bool alreadyIn = true;
                if (entity is null)
                {
                    entity = new Song()
                    {
                        AccountID = accountId,
                        FileName = song.Name,
                        StorageID = (int)StorageType.GoogleDrive,
                        StorageSongID = song.Id
                    };
                    alreadyIn = false;
                }
                if (updateMetadata)
                {
                    using var mStream = new MemoryStream();

                    await gdService.Files.Get(song.Id).DownloadAsync(mStream);
              
                    MetadataUtility.FillMetadata(entity, mStream);
                }
                if (!alreadyIn)
                {
                    await _dbContext.Songs.AddAsync(entity);
                }
            }
            await _dbContext.SaveChangesAsync();
        }
        private async Task UpdateSongsGDWebhook(int accountId, IList<Change> files, bool updateMetadata = false)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var gdDataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();

            var _dbContext = scope.ServiceProvider.GetRequiredService<MusicPlayerContext>();

            using var stream = new FileStream("googleDriveSecrets.json", FileMode.Open, FileAccess.Read);

            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                _gdScopes,
                accountId.ToString(),
                CancellationToken.None,
                gdDataStore);

            using var gdService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Garmusic",
            });

            foreach (var song in files)
            {
                if (!song.File.Name.EndsWith(".mp3"))
                {
                    continue;
                }

                var entity = await _dbContext.Songs.SingleOrDefaultAsync(s =>
                        s.FileName == song.File.Name &&
                        s.AccountID == accountId &&
                        s.StorageID == (int)StorageType.GoogleDrive);

                if (song.Removed == true)
                {
                    if (entity is not null)
                    {
                        _dbContext.Songs.Remove(entity);
                    }
                }
                else
                {
                    bool alreadyIn = true;
                    if (entity is null)
                    {
                        entity = new Song()
                        {
                            AccountID = accountId,
                            FileName = song.File.Name,
                            StorageID = (int)StorageType.GoogleDrive,
                            StorageSongID = song.FileId
                        };
                        alreadyIn = false;
                    }
                    if (updateMetadata)
                    {
                        using var mStream = new MemoryStream();

                        await gdService.Files.Get(song.FileId).DownloadAsync(mStream);

                        MetadataUtility.FillMetadata(entity, mStream);
                    }
                    if (!alreadyIn)
                    {
                        await _dbContext.Songs.AddAsync(entity);
                    }
                }
            }
            await _dbContext.SaveChangesAsync();
        }
        public async Task GoogleDriveWebhookMigrationAsync(string channelID)
        {
            var entity = await _dbContext.AccountStorages.SingleOrDefaultAsync(a => a.StorageID == (int)StorageType.GoogleDrive && a.JsonData.Contains(channelID));

            using var stream = new FileStream("googleDriveSecrets.json", FileMode.Open, FileAccess.Read);

            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                _gdScopes,
                entity.AccountID.ToString(),
                CancellationToken.None,
                _GDdataStore
                );

            using var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Garmusic"
            });

            GoogleDriveJson json = JsonConvert.DeserializeObject<GoogleDriveJson>(entity.JsonData);

            var changesRequest = service.Changes.List(json.StartPageToken);

            changesRequest.RestrictToMyDrive = true;
            changesRequest.IncludeItemsFromAllDrives = false;
            changesRequest.IncludeRemoved = true;
            changesRequest.SupportsAllDrives = false;

            var changeList = await changesRequest.ExecuteAsync();

            await UpdateSongsGDWebhook(entity.AccountID, changeList.Changes);

            if (_env.IsDevelopment())
            {
                _backgroundTaskQueue.EnqueueAsync(ct => UpdateSongsGDWebhook(entity.AccountID, changeList.Changes, true));
            }
            else
            {
                await UpdateSongsGDWebhook(entity.AccountID, changeList.Changes);
            }

            json.StartPageToken = changeList.NewStartPageToken;

            entity.JsonData = JsonConvert.SerializeObject(json);

            await _dbContext.SaveChangesAsync();
        }
    }
}
