using Dropbox.Api;
using Dropbox.Api.Files;
using Garmusic.Interfaces.Repositories;
using Garmusic.Models;
using Garmusic.Models.Entities;
using Garmusic.Models.EntitiesWatch;
using Garmusic.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Repositories
{
    public class SongRepository : ISongRepository
    {
        private readonly MusicPlayerContext _dbContext;
        public SongRepository(MusicPlayerContext context)
        {
            _dbContext = context;
        }

        public async Task AddSongToPlaylistAsync(int sID, int plID)
        {
            var song = await _dbContext.Songs.Include(s => s.Playlists).SingleOrDefaultAsync(s => s.Id == sID);
            var playlist = await _dbContext.Playlists.FindAsync(plID);

            song.Playlists.Add(playlist);

            await SaveAsync();
        }

        public async Task DeleteFromDbxAsync(int sID, int accountID)
        {

            var accountStorage = _dbContext.AccountStorages.Find(accountID, (int)StorageType.Dropbox);

            DropboxJson dbxJson = JsonConvert.DeserializeObject<DropboxJson>(accountStorage.JsonData);

            using var dbx = new DropboxClient(dbxJson.JwtToken);

            var entity = await _dbContext.Songs.FindAsync(sID);
            
            _dbContext.Songs.Remove(entity);

            await dbx.Files.DeleteV2Async(entity.StorageSongID);
            
            var files = await dbx.Files.ListFolderAsync("");

            dbxJson.Cursor = files.Cursor;

            accountStorage.JsonData = JsonConvert.SerializeObject(dbxJson);

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

            using var dbx = new DropboxClient(dbxJson.JwtToken);

            var song = await _dbContext.Songs.FindAsync(sID);

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
        public async Task<Song> PostToDbxAsync(IFormFile file, int accountId)
        {
            var accountStorage = _dbContext.AccountStorages.Find(accountId, (int)StorageType.Dropbox );
            
            DropboxJson dbxJson = JsonConvert.DeserializeObject<DropboxJson>(accountStorage.JsonData);

            using var dbx = new DropboxClient(dbxJson.JwtToken);

            var uploaded = await dbx.Files.UploadAsync(
                                                    "/" + file.FileName,
                                                    WriteMode.Add.Instance,
                                                    strictConflict: true,
                                                    body: file.OpenReadStream());
            Song song = new Song()
            {
                AccountID = accountId,
                FileName = file.FileName,
                StorageSongID = uploaded.Id,
                StorageID = (int)StorageType.Dropbox,
                Playlists = new List<Playlist>()
            };

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

        public async Task DeleteRangeFromDbxAsync(List<int> sIDs, int accountID)
        {
            var accountStorage = _dbContext.AccountStorages.Find(accountID, (int)StorageType.Dropbox);

            DropboxJson dbxJson = JsonConvert.DeserializeObject<DropboxJson>(accountStorage.JsonData);

            using var dbx = new DropboxClient(dbxJson.JwtToken);

            foreach (var sID in sIDs)
            {
                var entity = await _dbContext.Songs.FindAsync(sID);

                _dbContext.Songs.Remove(entity);
                
                await dbx.Files.DeleteV2Async(entity.StorageSongID);
            }

            var files = await dbx.Files.ListFolderAsync("");

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
