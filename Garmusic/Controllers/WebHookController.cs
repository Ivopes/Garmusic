using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
        public WebHookController(IConfiguration configuration)
        {
            _config = configuration;
        }
        [HttpGet("Dropbox")]
        public ActionResult<string> Dropbox(string challenge)
        {
            return Content(challenge);
        }
        [HttpPost("Dropbox")]
        public async Task<ActionResult<NotificationRequest>> Dropbox()
        {
            System.Diagnostics.Trace.TraceInformation("Dostal jsem hook");

            if (!Request.Headers.TryGetValue("X-Dropbox-Signature", out var signatureHeader))
            {
                return Unauthorized();
            }
            string signature = signatureHeader.FirstOrDefault();

            string body = await ResponseUtility.BodyStreamToStringAsync(Request.Body, (int)Request.ContentLength);

            NotificationRequest notificationRequest = JsonConvert.DeserializeObject<NotificationRequest>(body);

            using var sha = new HMACSHA256(Encoding.UTF8.GetBytes(_config.GetValue<string>("DropboxSecret")));

            var hashedBody = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(body))).Replace("-", "").ToLower();

            System.Diagnostics.Trace.TraceInformation("Jdu porovnat");
            System.Diagnostics.Trace.TraceInformation(body);
            System.Diagnostics.Trace.TraceInformation(hashedBody);
            System.Diagnostics.Trace.TraceInformation(signature);

            //var s = BitConverter.ToString(signature);

            if (hashedBody != signature)
            {
                return Unauthorized();
            }

            //var accounts = await _accountService.GetAllByStorageAccountIDAsync(data.list_folder.accounts);
            System.Diagnostics.Trace.TraceInformation("Uspech");
            //await _songService.MigrateSongs(notificationRequest.list_folder.accounts);
            return Ok(new { data = notificationRequest, sign = signatureHeader });
        }
    }
}
