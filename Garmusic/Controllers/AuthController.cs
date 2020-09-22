using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using API_BAK.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API_BAK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MusicPlayerContext _dbContext;
        public AuthController(MusicPlayerContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPost("Login")]
        public IActionResult Login([FromBody] Account account)
        {
            if (account == null)
            {
                return BadRequest("Invalid client request");
            }
            if(_dbContext.Accounts.Any(a => a.Username == account.Username && a.Password == account.Password))
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecretKey&&12345"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokenOptions = new JwtSecurityToken(
                    issuer: "http://localhost:5000",
                    audience: "http://localhost:5000",
                    claims: new List<Claim>(),
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                    );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return Ok(new { token = tokenString });
            }
            return Unauthorized();
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] Account account)
        {
            if (account == null)
            {
                return BadRequest("Invalid client request");
            }
            if (_dbContext.Accounts.Any(a => a.Email == account.Email))
            {
                return BadRequest("Email is already in use");
            }
            if(_dbContext.Accounts.Any(a => a.Username == account.Username))
            {
                return BadRequest("Username is already in use");
            }

            account.Created = DateTime.UtcNow;

            _dbContext.Accounts.Add(account);

            _dbContext.SaveChanges();

            return Ok(account);
        }
    }
}
