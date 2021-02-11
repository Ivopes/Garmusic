using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Models.Entities
{
    public class DbxOAuthResponse
    {
        public string Access_token { get; set; }
        public string Account_id { get; set; }
        public string Scope { get; set; }
        public string Token_type { get; set; }
        public string UId { get; set; }

    }
}
