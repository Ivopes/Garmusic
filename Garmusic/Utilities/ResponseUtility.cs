using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Garmusic.Utilities
{
    public static class ResponseUtility
    {
        public static async Task<string> BodyStreamToStringAsync(Stream stream, int length)
        {
            using var s = stream;

            byte[] buffer = new byte[length];

            await stream.ReadAsync(buffer, 0, length);

            string body = Encoding.UTF8.GetString(buffer);

            return body;
        }
    }
}
