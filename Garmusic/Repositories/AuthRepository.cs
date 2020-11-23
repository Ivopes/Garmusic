using Garmusic.Interfaces.Repositories;
using Garmusic.Models;
using Garmusic.Models.Entities;
using Garmusic.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Garmusic.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly MusicPlayerContext _dbContext;
        private readonly IConfiguration _config;
        public AuthRepository(MusicPlayerContext context, IConfiguration configuration)
        {
            _dbContext = context;
            _config = configuration;
        }
        public async Task<string> GetDropboxJwtAsync(int accountId)
        {
            var entity = await _dbContext.AccountStorages.FindAsync(new object[] { accountId, (int)StorageType.Dropbox});
            if (entity == null)
            {
                return "";
            }

            DropboxJson json = JsonConvert.DeserializeObject<DropboxJson>(entity.JsonData);

            if (string.IsNullOrEmpty(json.JwtToken))
            {
                return "";
            }

            return json.JwtToken;
        }
        public async Task<string> LoginAsync(Account account)
        {
            string token = "";

            Account entity = await _dbContext.Accounts.SingleOrDefaultAsync(acc => acc.Username == account.Username);

            if (entity != null)
            {
                byte[] passHash = PasswordUtility.HashPassword(account.Password, entity.PasswordSalt);

                if (Enumerable.SequenceEqual(entity.PasswordHash, passHash))
                {
                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("JwtSecretKey")));
                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                    var claims = new List<Claim>();
                    claims.Add(new Claim("uid", entity.AccountID.ToString()));

                    var tokenOptions = new JwtSecurityToken(
                        issuer: _config.GetValue<string>("ServerAdress"),
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(60),
                        signingCredentials: signinCredentials
                        );

                    token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                }
            }
            return token;
        }
        public async Task<string> RegisterAsync(Account account)
        {
            string response = "";

            if (await _dbContext.Accounts.AnyAsync(a => a.Email == account.Email))
            {
                response = "Email is already in use";
            }
            else
            {
                if (await _dbContext.Accounts.AnyAsync(a => a.Username == account.Username))
                {
                    response = "Username is already in use";
                }
                else
                {
                    account.Created = DateTime.UtcNow;

                    byte[] salt = PasswordUtility.GenerateSalt();

                    account.PasswordSalt = salt;

                    account.PasswordHash = PasswordUtility.HashPassword(account.Password, account.PasswordSalt);

                    await _dbContext.Accounts.AddAsync(account);

                    await _dbContext.SaveChangesAsync();
                }
            }

            return response;
        }
        public async Task RegisterDropboxAsync(int accountId, DropboxJson json)
        {
            AccountStorage entity = new AccountStorage()
            {
                AccountID = accountId,
                StorageID = (int)StorageType.Dropbox
            };

            string entityJson = JsonConvert.SerializeObject(json);

            entity.JsonData = entityJson;

            await _dbContext.AccountStorages.AddAsync(entity);

            await _dbContext.SaveChangesAsync();
        }
    }
}
