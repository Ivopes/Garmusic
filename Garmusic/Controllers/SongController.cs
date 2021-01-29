﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Garmusic.Models;
using Garmusic.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System;

namespace Garmusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
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
            int accountId = GetIdFromRequest();

            if (accountId == -1)
            {
                return BadRequest();
            }

            var result = await _songService.GetAllAsync(accountId);

            if(result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<Song>> PostAsync([FromForm] IFormFile file)
        {
            int accountId = GetIdFromRequest();

            if (accountId == -1)
            {
                return BadRequest();
            }

            Song song = null;

            try
            {
                song = await _songService.PostToDbxAsync(file, accountId);
            }
            catch(Exception ex)
            {
                return BadRequest("Oops! Something went wrong, please try again later. File may already exists");
            }

            return Ok(song);
        }
        [HttpDelete("{sID}")]
        public async Task<ActionResult> DeleteAsync(int sID)
        {
            int accountId = GetIdFromRequest();

            if (accountId == -1)
            {
                return BadRequest();
            }

            await _songService.DeleteFromDbxAsync(sID, accountId);

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
            int accountId = GetIdFromRequest();

            /*if (accountId == -1)
            {
                return BadRequest();
            }*/
            accountId = 1;
            var result = await _songService.GetFileByIdAsync(id, accountId);
            
            return File(result, "audio/mpeg");
        }
        [HttpGet("pl/{sID}/{plID}")]
        public async Task<ActionResult> AddSongToPlaylistAsync(int sID, int plID)
        {
            int accountId = GetIdFromRequest();

            if (accountId == -1)
            {
                return BadRequest();
            }

            await _songService.AddSongToPlaylistAsync(sID, plID);

            return Ok();
        }
        [HttpDelete("pl/{sID}/{plID}")]
        public async Task<ActionResult> RemovePlaylistsAsync(int sID, int plID)
        {
            int accountId = GetIdFromRequest();

            if (accountId == -1)
            {
                return BadRequest();
            }

            await _songService.RemovePlaylistAsync(sID, plID);

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
