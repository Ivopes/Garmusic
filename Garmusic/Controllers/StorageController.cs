using Garmusic.Interfaces.Services;
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
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;
        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Storage>>> GetAllAsync()
        {
            int accountId = JWTUtility.GetIdFromRequestHeaders(Request.Headers);

            if (accountId == -1)
            {
                return BadRequest();
            }

            var result = await _storageService.GetAllAsync();

            return Ok(result);
        }
    }
}
