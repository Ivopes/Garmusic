using Dropbox.Api;
using Dropbox.Api.Files;
using Garmusic.Interfaces.Repositories;
using Garmusic.Models;
using Garmusic.Models.Entities;
using Garmusic.Models.EntitiesWatch;
using Garmusic.Utilities;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garmusic.Repositories
{
    public class SongRepository : ISongRepository
    {
        private readonly MusicPlayerContext _dbContext;
        private readonly IDataStore _GDdataStore;

        private readonly string[] _gdScopes = { DriveService.Scope.Drive };

        public SongRepository(MusicPlayerContext context, IDataStore gDdataStore)
        {
            _dbContext = context;
            _GDdataStore = gDdataStore;
        }

        public async Task AddSongToPlaylistAsync(int sID, int plID)
        {
            var song = await _dbContext.Songs.Include(s => s.Playlists).SingleOrDefaultAsync(s => s.Id == sID);
            var playlist = await _dbContext.Playlists.FindAsync(plID);

            song.Playlists.Add(playlist);

            await SaveAsync();
        }

        public async Task DeleteAsync(int sID, int accountID)
        {

            var entity = await _dbContext.Songs.FindAsync(sID);

            _dbContext.Songs.Remove(entity);

            switch (entity.StorageID)
            {
                case (int)StorageType.Dropbox:
                {
                    var accountStorage = _dbContext.AccountStorages.Find(accountID, entity.StorageID);

                    DropboxJson dbxJson = JsonConvert.DeserializeObject<DropboxJson>(accountStorage.JsonData);

                    using var dbx = new DropboxClient(dbxJson.JwtToken);

                    await dbx.Files.DeleteV2Async(entity.StorageSongID);

                    var files = await dbx.Files.ListFolderAsync(string.Empty);

                    dbxJson.Cursor = files.Cursor;

                    accountStorage.JsonData = JsonConvert.SerializeObject(dbxJson);

                    break;
                }
                case (int)StorageType.GoogleDrive:
                {
                    using var stream = new FileStream("googleDriveSecrets.json", FileMode.Open, FileAccess.Read);

                    UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        _gdScopes,
                        accountID.ToString(),
                        CancellationToken.None,
                        _GDdataStore);

                    using var gdService = new DriveService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "Garmusic",
                    });

                    await gdService.Files.Delete(entity.StorageSongID).ExecuteAsync();

                    break;
                }
            }
            await SaveAsync();
        }

        public async Task<IEnumerable<Song>> GetAllAsync(int accountID)
        {
            return await _dbContext.Songs.Where(s => s.AccountID == accountID).Include(s => s.Playlists).ToListAsync();
        }

        public async Task<IEnumerable<SongWatch>> GetAllWatchAsync(int accountID)
        {
            var songs = await GetAllAsync(accountID);

            List<SongWatch> sw = new List<SongWatch>();

            foreach (var song in songs)
            {
                sw.Add(new SongWatch()
                {
                    Id = song.Id,
                    Name = song.FileName
                });
            }

            return sw;
        }

        public async Task<Song> GetByIdAsync(int sID)
        {
            return await _dbContext.Songs.FindAsync(sID);
        }

        public async Task<byte[]> GetDbxFileByIdAsync(int sID, int accountId)
        {
            var accountStorage = await _dbContext.AccountStorages.FindAsync(accountId, (int)StorageType.Dropbox);

            DropboxJson dbxJson = JsonConvert.DeserializeObject<DropboxJson>(accountStorage.JsonData);

            var song = await _dbContext.Songs.FindAsync(sID);

            using var dbx = new DropboxClient(dbxJson.JwtToken);

            var file = await dbx.Files.DownloadAsync(song.StorageSongID);

            var bytes = await file.GetContentAsByteArrayAsync();

            return bytes;
        }
        public async Task<bool> CanModifyAsync(int accountID, int sID, int plID)
        {
            var song = await _dbContext.Songs.FindAsync(sID);

            if (song is null)
            {
                return false;
            }

            var pl = await _dbContext.Playlists.FindAsync(plID);

            if (pl is null)
            {
                return false;
            }
            
            return song.AccountID == accountID && pl.AccountID == accountID;
        }
        public async Task PostAsync(Song song)
        {
            await _dbContext.Songs.AddAsync(song);
        }
        public async Task<Song> PostAsync(IFormFile file, int accountID, int storageID)
        {
            Song song = null;
            switch (storageID)
            {
                case (int)StorageType.Dropbox:
                    {
                        var accountStorage = _dbContext.AccountStorages.Find(accountID, (int)StorageType.Dropbox);

                        DropboxJson dbxJson = JsonConvert.DeserializeObject<DropboxJson>(accountStorage.JsonData);

                        using var dbx = new DropboxClient(dbxJson.JwtToken);

                        var uploaded = await dbx.Files.UploadAsync(
                                                                "/" + file.FileName,
                                                                WriteMode.Add.Instance,
                                                                strictConflict: true,
                                                                body: file.OpenReadStream());
                        song = new Song()
                        {
                            AccountID = accountID,
                            FileName = file.FileName,
                            StorageSongID = uploaded.Id,
                            StorageID = storageID,
                            Playlists = new List<Playlist>()
                        };

                        break;
                    }
                case (int)StorageType.GoogleDrive:
                    {
                        using var s = new FileStream("googleDriveSecrets.json", FileMode.Open, FileAccess.Read);

                        UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(s).Secrets,
                            _gdScopes,
                            accountID.ToString(),
                            CancellationToken.None,
                            _GDdataStore);

                        using var gdService = new DriveService(new BaseClientService.Initializer()
                        {
                            HttpClientInitializer = credential,
                            ApplicationName = "Garmusic",
                        });

                        var ids = gdService.Files.GenerateIds();

                        ids.Count = 1;
                        
                        Google.Apis.Drive.v3.Data.File f = new Google.Apis.Drive.v3.Data.File();

                        f.Id = (await ids.ExecuteAsync()).Ids[0];
                        f.Name = file.FileName;

                        await gdService.Files.Create(f, file.OpenReadStream(), "audio/mpeg").UploadAsync();

                        song = new Song()
                        {
                            AccountID = accountID,
                            FileName = file.FileName,
                            StorageSongID = f.Id,
                            StorageID = storageID,
                            Playlists = new List<Playlist>()
                        };

                        break;
                    }
            }
            using Stream stream = file.OpenReadStream();

            MetadataUtility.FillMetadata(song, stream);

            await PostAsync(song);

            await SaveAsync();

            return song;
        }
        public async Task RemovePlaylistAsync(int sID, int plID)
        {
            var song = await _dbContext.Songs.Include(s => s.Playlists).SingleOrDefaultAsync(s => s.Id == sID);
            var playlist = await _dbContext.Playlists.FindAsync(plID);

            song.Playlists.Remove(playlist);

            await SaveAsync();
        }
        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(List<int> sIDs, int accountID)
        {
            var accountStorage = _dbContext.AccountStorages.Find(accountID, (int)StorageType.Dropbox);

            DropboxJson dbxJson = JsonConvert.DeserializeObject<DropboxJson>(accountStorage.JsonData);

            using var dbx = new DropboxClient(dbxJson.JwtToken);

            using var stream = new FileStream("googleDriveSecrets.json", FileMode.Open, FileAccess.Read);

            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                                GoogleClientSecrets.Load(stream).Secrets,
                                _gdScopes,
                                accountID.ToString(),
                                CancellationToken.None,
                                _GDdataStore);

            using var gdService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Garmusic",
            });

            foreach (var sID in sIDs)
            {
                var entity = await _dbContext.Songs.FindAsync(sID);
                _dbContext.Songs.Remove(entity);
                switch (entity.StorageID)
                {
                    case (int)StorageType.Dropbox:
                        {
                            await dbx.Files.DeleteV2Async(entity.StorageSongID);
                            break;
                        }
                    case (int)StorageType.GoogleDrive:
                        {
                            await gdService.Files.Delete(entity.StorageSongID).ExecuteAsync();
                            break;
                        }
                }
            }
            var files = await dbx.Files.ListFolderAsync(string.Empty);
            dbxJson.Cursor = files.Cursor;
            accountStorage.JsonData = JsonConvert.SerializeObject(dbxJson);

            await SaveAsync();
        }

        public async Task<bool> CanModifyAsync(int accountID, int sID)
        {
            var song = await _dbContext.Songs.FindAsync(sID);

            if (song is null)
            {
                return false;
            }

            return song.AccountID == accountID;
        }
    }
}
