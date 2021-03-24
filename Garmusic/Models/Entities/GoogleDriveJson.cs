using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Models.Entities
{
    public class GoogleDriveJson
    {
        public string Token { get; set; }
        public string StartPageToken { get; set; }
        public string ChannelId { get; set; }
    }
}
