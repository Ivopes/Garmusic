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
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Garmusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMigrationService _migService;
        private readonly IConfiguration _config;
        private readonly IDataStore _dataStore;
        public AuthController(IAuthService authService, IMigrationService migrationService, IConfiguration configuration, IDataStore dataStore)
        {
            _authService = authService;
            _migService = migrationService;
            _config = configuration;
            _dataStore = dataStore;
        }
        [HttpPost("Login")]
        public async Task<ActionResult> LoginAsync([FromBody] Account account)
        {
            if (account is null)
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
        [HttpPost("Login/Watch")]
        public async Task<ActionResult> LoginWatchAsync([FromBody] Account account)
        {
            if (account is null)
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
        public async Task<ActionResult> RegisterAsync([FromBody] Account account)
        {
            if (account is null)
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
        [HttpGet("dbx/{dbxCode}")]
        public async Task<ActionResult> RegisterDropboxAsync(string dbxCode)
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if(accountId == -1)
            {
                return BadRequest();
            }

            using var client = new HttpClient();

            string dbxKeys = _authService.GetDropboxKeys();

            var dict = new Dictionary<string, string>();

            dict.Add("grant_type", "authorization_code");
            dict.Add("code", dbxCode);
            dict.Add("redirect_uri", _config.GetValue<string>("DropboxRedirectURL"));

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", dbxKeys);

            var req = new HttpRequestMessage(HttpMethod.Post, "https://api.dropbox.com/1/oauth2/token") 
            { 
                Content = new FormUrlEncodedContent(dict) 
            };

            var response = await client.SendAsync(req);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            var resBody = await response.Content.ReadAsStringAsync();
            
            var db = JsonConvert.DeserializeObject<DbxOAuthResponse>(resBody);

            var json = new DropboxJson()
            {
                Cursor = "",
                DropboxID = db.Account_id,
                JwtToken = db.Access_token
            };

            await _authService.RegisterDropboxAsync(accountId, json);

            await _migService.DropboxMigrationAsync(accountId); 

            return Ok();
        }
        [HttpDelete("dbx")]
        public async Task<ActionResult<string>> SignOutDbx()
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            await _authService.SignOutDbx(accountId);
            
            return Ok();
        }
        [HttpDelete("gd")]
        public async Task<ActionResult<string>> SignOutGoogleDrive()
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            await _authService.SignOutGoogleDrive(accountId);

            return Ok();
        }
        [HttpGet("gd")]
        public async Task<ActionResult> SignInGoogleDrive()
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            string[] Scopes = { DriveService.Scope.DriveReadonly };

            using var stream = new FileStream("googleDriveSecrets.json", FileMode.Open, FileAccess.Read);

            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets, 
                Scopes, 
                accountId.ToString(), 
                CancellationToken.None,
                _dataStore);

            //UserCredential c = new UserCredential();

            /*var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Garmusic",
            });*/

            //var files = await service.Files.List().ExecuteAsync();

            //await _authService.RegisterGoogleDriveAsync(accountId, await credential.GetAccessTokenForRequestAsync());

            await _migService.GoogleDriveMigrationAsync(accountId);

            return Ok();
        }
    }
}
