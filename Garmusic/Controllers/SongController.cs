using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Garmusic.Models;
using Garmusic.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System;
using Garmusic.Utilities;
using ATL;
using System.IO;
using Garmusic.Models.EntitiesWeb;
using Microsoft.AspNetCore.Authorization;

namespace Garmusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SongController : ControllerBase
    {       

        private readonly ISongService _songService;
        public SongController(ISongService songService)
        {
            _songService = songService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetAllAsync()
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            var songs = await _songService.GetAllAsync(accountId);

            if (songs is null)
            {
                return NotFound();
            }

            var result = new List<SongWeb>();

            foreach (var song in songs)
            {
                result.Add(new SongWeb()
                { 
                    Id = song.Id,
                    FileName = song.FileName,
                    Name = song.Name,
                    Author = song.Author,
                    LengthSec = song.LengthSec,
                    Playlists = song.Playlists,
                    StorageID = song.StorageID
                });
            }

            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<Song>> PostAsync([FromForm] IFormFile file, [FromForm] int storageID)
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            Song song = null;

            try
            {
                song = await _songService.PostAsync(file, accountId, storageID);
            }
            catch(Exception ex)
            {
                return BadRequest("Oops! Something went wrong, please try again later");
            }

            return Ok(song);
        }
        [HttpDelete("{sID}")]
        public async Task<ActionResult> DeleteAsync(int sID)
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            if (!await _songService.CanModifyAsync(accountId, sID))
            {
                return Unauthorized();
            }

            await _songService.DeleteAsync(sID, accountId);

            return Ok();
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteRangeAsync([FromBody] List<int> sIDs)
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            foreach (var id in sIDs)
            {
                if (!await _songService.CanModifyAsync(accountId, id))
                {
                    return Unauthorized();
                }
            }

            await _songService.DeleteRangeAsync(sIDs, accountId);

            return Ok();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Song>> GetByIdAsync(int id)
        {
            return await _songService.GetByIdAsync(id);
        }
        [HttpGet("file/{id}")]
        public async Task<ActionResult> GetFileByIdAsync(int id)
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }
            var result = await _songService.GetFileByIdAsync(id, accountId);
            
            return File(result, "audio/mpeg");
        }
        [HttpGet("pl/{sID}/{plID}")]
        public async Task<ActionResult> AddSongToPlaylistAsync(int sID, int plID)
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            if (!await _songService.CanModifyAsync(accountId, sID, plID))
            {
                return Unauthorized();
            }

            await _songService.AddSongToPlaylistAsync(sID, plID);

            return Ok();
        }
        [HttpDelete("pl/{sID}/{plID}")]
        public async Task<ActionResult> RemovePlaylistsAsync(int sID, int plID)
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            if (!await _songService.CanModifyAsync(accountId, sID, plID))
            {
                return Unauthorized();
            }

            await _songService.RemovePlaylistAsync(sID, plID);

            return Ok();
        }
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] Song song)
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            await _songService.PutAsync(song);

            return Ok();
        }
    }
}
