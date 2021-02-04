using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Utilities
{
    public static class JWTUtility
    {
        /// <summary>
        /// Extract the ID of account from JWT token which is sent in request header
        /// </summary>
        /// <returns>ID of account</returns>
        public static int GetIdFromRequestHeaders(IHeaderDictionary headers)
        {
            int accountId = -1;
            if (headers.TryGetValue("Authorization", out var token))
            {
                // Extract the "Bearer" from JWT header
                var t = new JwtSecurityTokenHandler().ReadJwtToken(token[0].Substring(7));
                var claims = t.Payload.Claims;
                accountId = int.Parse(claims.FirstOrDefault(b => b.Type == "uid").Value);
            }
            return accountId;
        }
    }
}
