using System.Collections.Generic;
using Garmusic.Models;
using Microsoft.AspNetCore.Mvc;
using Garmusic.Utilities;
using Garmusic.Interfaces.Services;
using System.Threading.Tasks;
using Garmusic.Models.Entities;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth.OAuth2.Flows;
using System.Threading;
using System;
using System.Web;

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
            
        private readonly string[] _gdScopes = { DriveService.Scope.Drive };
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

            if (jwtToken == string.Empty)
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

            if (jwtToken == string.Empty)
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

            if (response == string.Empty)
            {
                return Ok();
            }

            return BadRequest(response);
        }
        [HttpGet("dbx/{dbxCode}")]
        [Authorize]
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
                Cursor = string.Empty,
                DropboxID = db.Account_id,
                JwtToken = db.Access_token
            };

            await _authService.RegisterDropboxAsync(accountId, json);

            await _migService.DropboxMigrationAsync(accountId); 

            return Ok();
        }
        [HttpDelete("dbx")]
        [Authorize]
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
        [Authorize]
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
        [HttpGet("gd/url")]
        [Authorize]
        public async Task<ActionResult<string>> GetSignInGDUrl()
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            using var stream = new FileStream("googleDriveSecrets.json", FileMode.Open, FileAccess.Read);

            IAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecretsStream = stream,
                Scopes = _gdScopes,
                DataStore = _dataStore
            });

            var request = flow.CreateAuthorizationCodeRequest(_config.GetValue<string>("GDRedirectURL"));

            string url = request.Build().ToString();

            return Ok(url);
        }
        [HttpGet("gd/{gdCode}")]
        [Authorize]
        public async Task<ActionResult> RegisterGoogleDrive(string gdCode)
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            using var stream = new FileStream("googleDriveSecrets.json", FileMode.Open, FileAccess.Read);

            IAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecretsStream = stream,
                Scopes = _gdScopes,
                DataStore = _dataStore
            });


            string code = HttpUtility.UrlDecode(gdCode);

            var response = await flow.ExchangeCodeForTokenAsync(
                accountId.ToString(),
                code,
                _config.GetValue<string>("GDRedirectURL"),
                CancellationToken.None
                );

            await _migService.GoogleDriveMigrationAsync(accountId);

            return Ok();
        }
    }
}
