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

namespace Garmusic.Controllers
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
        public IActionResult Login([FromBody] Account acc)
        {
            if (acc == null)
            {
                return BadRequest("Invalid client request");
            }

            Account account = _dbContext.Accounts.SingleOrDefault(a => a.Username == acc.Username);

            if(account == null)
            {
                return Unauthorized();
            }

            byte[] passHash = PasswordUtility.HashPassword(acc.Password, account.PasswordSalt);
            
            if(Enumerable.SequenceEqual(account.PasswordHash, passHash))
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecretKey&&12345"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                
                var claims = new List<Claim>();
                claims.Add(new Claim("uid", account.AccountID.ToString()));

                var tokenOptions = new JwtSecurityToken(
                    issuer: "http://localhost:5000",
                    audience: "http://localhost:5000",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(60),
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

            byte[] salt = PasswordUtility.GenerateSalt();

            account.PasswordSalt = salt;

            account.PasswordHash = PasswordUtility.HashPassword(account.Password, account.PasswordSalt);

            _dbContext.Accounts.Add(account);

            _dbContext.SaveChanges();

            return Ok(account);
        }
    }
}
