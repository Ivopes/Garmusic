using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Garmusic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Garmusic.Utilities;
using Garmusic.Services.Dependencies;
using Garmusic.Interfaces.Services;
using System.Threading.Tasks;
using Garmusic.Models.Entities;

namespace Garmusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MusicPlayerContext _dbContext;
        private readonly IAuthService _authService;
        private readonly IMigrationService _migService;
        public AuthController(MusicPlayerContext dbContext, IAuthService authService, IMigrationService migrationService)
        {
            _dbContext = dbContext;
            _authService = authService;
            _migService = migrationService;
        }
        [HttpPost("Login")]
        public async Task<ActionResult> LoginAsync([FromBody] Account account)
        {
            if (account == null)
            {
                return BadRequest("Invalid client request");
            }

            string jwtToken = await _authService.LoginAsync(account);

            if (jwtToken == "")
            {
                return Unauthorized();
            }

            return Ok(new { token = jwtToken });
        }
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] Account account)
        {
            if (account == null)
            {
                return BadRequest("Invalid client request");
            }

            string response = await _authService.RegisterAsync(account);

            if (response == "")
            {
                return Ok();
            }

            return BadRequest(response);
        }
        [HttpPost("registerDropbox")]
        public async Task<ActionResult> RegisterDropboxAsync([FromBody] DropboxJson json)
        {
            int accountId = GetIdFromRequest();
            if(accountId == -1)
            {
                return BadRequest();
            }

            await _authService.RegisterDropboxAsync(accountId, json);

            await _migService.DropboxMigrationAsync(accountId); 

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
