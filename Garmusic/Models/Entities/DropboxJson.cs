using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Models.Entities
{
    public class DropboxJson
    {
        public string Cursor { get; set; }
        public string JwtToken { get; set; }
        public string DropboxID { get; set; }
    }
}
