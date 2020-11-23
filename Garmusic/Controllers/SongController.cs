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
using Microsoft.IdentityModel.Protocols;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

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
        private readonly IConfiguration _config;
        public SongController(MusicPlayerContext dbContext, ISongService songService, IAccountService accountService, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _songService = songService;
            _accountService = accountService;
            _config = configuration;
        }
        // GET: api/Song
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetAllAsync()
        {
            int accountId = GetIdFromRequest();

            if (accountId == -1)
            {
                return BadRequest();
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
        public async Task<ActionResult> PostAsync([FromForm] IFormFile file)
        {
            int accountId = GetIdFromRequest();

            if (accountId == -1)
            {
                return BadRequest();
            }

            try
            {
                await _songService.PostToDbxAsync(file, accountId);
            }
            catch(Exception ex)
            {
                return BadRequest("Oops! Something went wrong, please try again later. File may already exists");
            }

            return Ok();
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
        [HttpGet("database/s")]
        public Task<IEnumerable<Song>> GetSFromDatabase()
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
            if(!Request.Headers.TryGetValue("X-Dropbox-Signature", out var signatureHeader))
            {
                return Unauthorized();
            }
            string signature = signatureHeader.FirstOrDefault();

            string body = await ResponseUtility.BodyStreamToStringAsync(Request.Body, (int) Request.ContentLength);

            var bodyBytes = Encoding.UTF8.GetBytes("ahoj");

            using var sha = new HMACSHA256(Encoding.UTF8.GetBytes(_config.GetValue<string>("DropboxSecret")));

            var hashedBody = sha.ComputeHash(Encoding.UTF8.GetBytes(body));
            var b = Encoding.UTF8.GetString(hashedBody);
            var a = System.Convert.ToBase64String(hashedBody);
            if(!Enumerable.SequenceEqual(hashedBody, sha.Hash))
            {
                return Unauthorized();
            }

            NotificationRequest notificationRequest = JsonConvert.DeserializeObject<NotificationRequest>(body);
            //var accounts = await _accountService.GetAllByStorageAccountIDAsync(data.list_folder.accounts);
            
            //await _songService.MigrateSongs(notificationRequest.list_folder.accounts);
            return Ok();
        }
        private int GetIdFromRequest()
        {
            int accountId = -1;
            if (Request.Headers.TryGetValue("Authorization", out var token))
            {
                var a = new JwtSecurityTokenHandler().ReadJwtToken(token[0].Substring(7));
                var b = a.Payload.Claims;
                accountId = int.Parse(b.FirstOrDefault(b => b.Type == "uid").Value);
            }
            return accountId;
        }
    }
}
