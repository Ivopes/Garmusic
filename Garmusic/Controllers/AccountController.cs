using Garmusic.Interfaces.Services;
using Garmusic.Models;
using Garmusic.Models.Entities;
using Garmusic.Utilities;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            var result  = await _accountService.GetByIdAsync(accountId);

            return result is not null ? Ok(result) : NotFound();
        }
    }
}
