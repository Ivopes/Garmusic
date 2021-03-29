using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Garmusic.Interfaces.Services;
using Garmusic.Models.Entities;
using Garmusic.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Garmusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAccountService _accService;
        private readonly IMigrationService _migService;
        public WebHookController(IConfiguration configuration, IAccountService accountService, IMigrationService migrationService)
        {
            _config = configuration;
            _accService = accountService;
            _migService = migrationService;
        }
        /// <summary>
        /// echoes request from Dbx to make Dbx sure we are ready to accept it
        /// </summary>
        /// <param name="challenge"></param>
        /// <returns>echo with request</returns>
        [HttpGet("Dropbox")]
        public ActionResult<string> Dropbox(string challenge)
        {
            return Content(challenge);
        }
        [HttpPost("Dropbox")]
        public async Task<ActionResult<NotificationRequest>> Dropbox()
        {   
            if (!Request.Headers.TryGetValue("X-Dropbox-Signature", out var signatureHeader))
            {
                return Unauthorized();
            }
            string body = await ResponseUtility.BodyStreamToStringAsync(Request.Body, (int)Request.ContentLength);

            using var sha = new HMACSHA256(Encoding.UTF8.GetBytes(_config.GetValue<string>("DropboxSecret")));

            var hashedBody = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(body))).Replace("-", string.Empty).ToLower();

            string signature = signatureHeader.FirstOrDefault();

            if (hashedBody != signature)
            {
                return Unauthorized();
            }

            NotificationRequest data = JsonConvert.DeserializeObject<NotificationRequest>(body);

            await _migService.DropboxWebhookMigrationAsync(data.list_folder.accounts);

            return Ok();
        }
        [HttpPost("GoogleDrive")]
        public async Task<ActionResult<NotificationRequest>> GoogleDrive()
        {
            // Check uuid

            //string body = await ResponseUtility.BodyStreamToStringAsync(Request.Body, (int)Request.ContentLength);


            var number = Request.Headers["X-Goog-Message-Number"];

            if (int.TryParse(number, out int res))
            {
                // Ignore Sync request
                if (res == 1)
                {
                    return Ok();
                }

                var channelID = Request.Headers["X-Goog-Channel-ID"];

                try
                {

                    await _migService.GoogleDriveWebhookMigrationAsync(channelID.ToString());
                }
                catch(Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    Trace.WriteLine(ex.InnerException);
                    //Trace.WriteLine(ex.)
                }

                return Ok();
            }

            return BadRequest();
        }
    }
}
