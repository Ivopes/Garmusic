﻿using Microsoft.AspNetCore.Mvc;
using Garmusic.Models;
using System.Threading.Tasks;
using Garmusic.Interfaces.Services;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Garmusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;
        public PlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }
        // GET: api/<PlaylistController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Playlist>>> GetAllAsync()
        {
            int accountId = GetIdFromRequest();

            if(accountId == -1)
            {
                return BadRequest();
            }

            return Ok(await _playlistService.GetAllAsync(accountId));
        }

        // GET api/<PlaylistController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PlaylistController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Playlist playlist)
        {
            int accountId = GetIdFromRequest();

            if (accountId == -1)
            {
                return BadRequest();
            }

            playlist.AccountID = accountId;

            await _playlistService.PostAsync(playlist);
            
            return Ok();
        }

        // PUT api/<PlaylistController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PlaylistController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
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
