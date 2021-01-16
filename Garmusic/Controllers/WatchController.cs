using Garmusic.Interfaces.Services;
using Garmusic.Models.EntitiesWatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            /*int accountId = GetIdFromRequest();

             if (accountId == -1)
             {
                 return BadRequest();
             }*/
            int accountId = 1;

            var songs = await _songService.GetAllWatchAsync(accountId);
            var playlists = await _playlistService.GetAllWatchAsync(accountId);

            if (songs == null || playlists == null)
            {
                return NotFound();
            }
            //var a = songs.ToList();
            //a.RemoveAt(0);
            var a = new List<SongWatch>();
            a.Add(new SongWatch() { Id = 5 });
            a.Add(new SongWatch() { Id = 6 });
            a.Add(new SongWatch() { Id = 7 });
            a.Add(new SongWatch() { Id = 8 });
            a.Add(new SongWatch() { Id = 9 });
            //var b = playlists.ToList();
            var b = new List<PlaylistWatch>();
            //b.RemoveAt(2);
            b.Add(new PlaylistWatch() { Id = 0, SongsIds = new List<int>() });
            b.Add(new PlaylistWatch() { Id = -1 });
            b.Add(new PlaylistWatch() { Id = -2 });
            b.Add(new PlaylistWatch() { Id = -3 });
            b.Add(new PlaylistWatch() { Id = -4 });

            var result = new { songs = songs, playlists = playlists};
            
            return Ok(result);
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
