using Garmusic.Interfaces.Services;
using Garmusic.Models;
using Garmusic.Models.EntitiesWatch;
using Garmusic.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garmusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WatchController : ControllerBase
    {
        private readonly ISongService _songService;
        private readonly IPlaylistService _playlistService;
        public WatchController(ISongService songService, IPlaylistService playlistService)
        {
            _songService = songService;
            _playlistService = playlistService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

             if (accountId == -1)
             {
                 return BadRequest();
             }
            //int accountId = 1;

            var songs = await _songService.GetAllWatchAsync(accountId);
            var playlists = await _playlistService.GetAllWatchAsync(accountId);

            if (songs is null || playlists is null)
            {
                return NotFound();
            }

            var result = new { songs = songs, playlists = playlists};
            
            return Ok(result);
        }
        [HttpPut]
        public async Task<ActionResult> Put()
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }
            //int accountId = 1;

            // Method cant handle parameter
            // This is a replacement
            using StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);

            string json = await reader.ReadToEndAsync();

            if(json.Length <= 2)
            {
                return BadRequest();
            }

            var parsedJson = json.Remove(json.Length - 1).Substring(json.IndexOf(':') + 1);

            var playlistsWatch = JsonConvert.DeserializeObject<List<PlaylistWatch>>(parsedJson);

            var playlists = new List<Playlist>();

            foreach (var plw in playlistsWatch)
            {
                playlists.Add(new Playlist()
                {
                    Id = plw.Id,
                    Sync = plw.Sync
                });
            }

            if (playlists.Count == 0)
            {
                return BadRequest();
            }

            await _playlistService.UpdateSyncAsync(playlists);

            return Ok();
        }
    }
}
