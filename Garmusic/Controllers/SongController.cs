using System.Threading.Tasks;
using Dropbox.Api;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Dropbox.Api.Files;
using Microsoft.AspNetCore.Http;
using Garmusic.Models;
using Microsoft.EntityFrameworkCore;
using Garmusic.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;
using Garmusic.Models.Entities;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Garmusic.Utilities;

namespace Garmusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SongController : ControllerBase
    {       

        private readonly string _conn = "sl.AkBy8-dZtjfeV3-UOS8wUHQigMAoS-GAimeMZiOFJK_7snvydVGQ3C3Zld9NnmADepXKqVH-XmOmzeB_rlc6gxMem5C6Tlqo9s6W2TvPjQbZqoXSvE4dRkJbOCjZL47dZ9LFvB8";
        private readonly MusicPlayerContext _dbContext;
        private readonly ISongService _songService;
        private readonly IAccountService _accountService;
        public SongController(MusicPlayerContext dbContext, ISongService songService, IAccountService accountService)
        {
            _dbContext = dbContext;
            _songService = songService;
            _accountService = accountService;
        }
        // GET: api/Song
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetAll()
        {
            /*byte[] bytes;
            string fileLocation = "Assets/sample.mp3";
            using (var reader = new BinaryReader(new FileStream(fileLocation, FileMode.Open)))
            {
                long bytesNum = new FileInfo(fileLocation).Length;
                bytes = reader.ReadBytes((int) bytesNum);
            }

            return File(bytes, "audio/mpeg", "stahnuty.mp3");*/
            //return await _dbContext.Songs.ToListAsync();
            int accountId = -1;
            if(Request.Headers.TryGetValue("Authorization", out var token))
            {
                var a = new JwtSecurityTokenHandler().ReadJwtToken(token[0].Substring(7));
                var b = a.Payload.Claims;
                
                accountId = int.Parse(b.FirstOrDefault(b => b.Type == "uid").Value);
            }
            var result = await _songService.GetAllAsync(accountId);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        // POST: api/Song
        [HttpPost]
        public async Task<bool> Post([FromForm]IFormFile file)
        {
            
            using var dbx = new DropboxClient(_conn);

            var uploaded = await dbx.Files.UploadAsync(
                                        "/" + file.FileName,
                                        WriteMode.Add.Instance,
                                        autorename: true,
                                        body: file.OpenReadStream());
            
            return uploaded != null;
        }
        // PUT: api/Mp3/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        [HttpGet("info")]
        public async Task<FileResult> GetDropboxMp3Async()
        {

            using var dbx = new DropboxClient(_conn);

            var file = await dbx.Files.DownloadAsync("/sample.mp3");

            var bytes = await file.GetContentAsByteArrayAsync();

            return File(bytes, "audio/mpeg", "dropbox.mp3");
        }
        [HttpGet("all")]
        public async Task<IList<string>> GetAllAsync()
        {
            using var dbx = new DropboxClient(_conn);

            var files = await dbx.Files.ListFolderAsync(string.Empty);

            List<string> names = new List<string>();

            foreach (var file in files.Entries)
            {
                names.Add(file.Name);
            }
         
            return names;
        }
        [HttpGet("database/s")]
        public async Task<IEnumerable<Song>> GetSFromDatabase()
        {
            //return _dbContext.Songs.Include(s => s.Account);
            //return await _songService.GetAllAsync();
            throw new NotImplementedException();
        }
        [HttpGet("database/p")]
        public IEnumerable<Playlist> GetPFromDatabase()
        {
            return _dbContext.Playlists.Include(p => p.Songs).ThenInclude(ps => ps.Song);
        }
        [HttpGet("database/a")]
        public IEnumerable<Account> GetAFromDatabase()
        {
            var list = _dbContext.Accounts.Include(a => a.Songs);
            /*foreach (var item in list)
            {
                foreach (var song in item.Songs)
                {
                    song.Account = null;
                }
            }*/
            return list;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Song>> GetByIdAsync(int id)
        {
            return await _songService.GetByIdAsync(id);
        }
        [HttpPost("migrate")]
        public async Task<ActionResult> MigrateSongs()
        {
            string body = await ResponseUtility.BodyStreamToStringAsync(Request.Body, (int) Request.ContentLength);

            NotificationRequest notificationRequest = JsonConvert.DeserializeObject<NotificationRequest>(body);

            var accounts = await _accountService.GetAllByStorageAccountIDAsync(notificationRequest.list_folder.accounts);

            //await _songService.MigrateSongs(notificationRequest.list_folder.accounts);
            return Ok();
        }
    }
}
