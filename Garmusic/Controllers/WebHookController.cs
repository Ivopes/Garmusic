﻿using System;
using System.Collections.Generic;
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
            string signature = signatureHeader.FirstOrDefault();

            string body = await ResponseUtility.BodyStreamToStringAsync(Request.Body, (int)Request.ContentLength);

            using var sha = new HMACSHA256(Encoding.UTF8.GetBytes(_config.GetValue<string>("DropboxSecret")));

            var hashedBody = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(body))).Replace("-", "").ToLower();

            if (hashedBody != signature)
            {
                return Unauthorized();
            }

            NotificationRequest data = JsonConvert.DeserializeObject<NotificationRequest>(body);

            var accounts = await _accService.GetAllByStorageAccountIDAsync(data.list_folder.accounts);

            await _migService.DropboxMigrateAsync(accounts);

            //await _songService.MigrateSongs(notificationRequest.list_folder.accounts);
            return Ok();
        }
    }
}