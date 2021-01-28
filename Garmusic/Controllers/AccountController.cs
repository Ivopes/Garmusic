﻿using Garmusic.Interfaces.Services;
using Garmusic.Models;
using Garmusic.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpGet]
        public async Task<ActionResult<AccountWeb>> GetByIdAsync()
        {
            int accountId = GetIdFromRequest();

            if (accountId == -1)
            {
                return BadRequest();
            }

            var result  = await _accountService.GetByIdAsync(accountId);

            return result is not null ? Ok(result) : NotFound();
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
