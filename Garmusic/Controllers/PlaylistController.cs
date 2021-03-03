using Microsoft.AspNetCore.Mvc;
using Garmusic.Models;
using System.Threading.Tasks;
using Garmusic.Interfaces.Services;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Garmusic.Models.EntitiesWatch;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using Garmusic.Utilities;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Garmusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlaylistController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;
        public PlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Playlist>>> GetAllAsync()
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            var result = await _playlistService.GetAllAsync(accountId);

            return Ok(result);
        }
        [HttpGet("{plId}")]
        public async Task<ActionResult<Playlist>> GetByIdAsync(int plId)
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            var pls = await _playlistService.GetAllAsync(accountId);

            var result = pls.FirstOrDefault(pls => pls.Id == plId);

            return Ok(result);
        }
        [HttpGet("songs/{id}")]
        public async Task<ActionResult<IEnumerable<Song>>> GetSongsById(int id)
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest(null);
            }

            var result = await _playlistService.GetSongsByPlIdAsync(id);

            return Ok(result);

        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Playlist playlist)
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            playlist.AccountID = accountId;

            await _playlistService.PostAsync(playlist);
            
            return Ok();
        }
        [HttpDelete("{pID}")]
        public async Task<ActionResult> DeleteAsync(int pID)
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            if (!await _playlistService.CanModifyAsync(accountId, pID))
            {
                return Unauthorized();
            }

            await _playlistService.RemoveAsync(pID);

            return Ok();
        }
    }
}
