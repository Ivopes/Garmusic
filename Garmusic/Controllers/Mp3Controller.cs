using System;
using System.IO;
using System.Threading.Tasks;
using Dropbox.Api;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using Dropbox.Api.Files;
using Dropbox.Api.Stone;
using Microsoft.AspNetCore.Http;

namespace Garmusic.Controllers
{
    public class YourDataModel
    {
        public IFormFile File { get; set; }
        // whatever other properties you need
    }
    [Route("api/[controller]")]
    [ApiController]
    public class Mp3Controller : ControllerBase
    {       

        private readonly string _conn = "sl.AjYf6yW4uyK_ON7ZiR0O5b-S2LelFVSYK3t-MJHaVQKguHH_y--PPqFUUP-wycA0Z1g_5_sQAx5yWOirxxpJr9AWG4NN1TpnXuPQontmuyiIfb-hHku6SQs12CqAPMTKUPvFqcY";

        // GET: api/Mp3
        [HttpGet]
        public FileResult GetMp3()
        {
            byte[] bytes;
            string fileLocation = "Assets/sample.mp3";
            using (var reader = new BinaryReader(new FileStream(fileLocation, FileMode.Open)))
            {
                long bytesNum = new FileInfo(fileLocation).Length;
                bytes = reader.ReadBytes((int) bytesNum);
            }

            return File(bytes, "audio/mpeg", "stahnuty.mp3");
        }
        // POST: api/Mp3
        [HttpPost]
        public async Task<bool> Post([FromForm]IFormFile file)
        {
            
            using var dbx = new DropboxClient(_conn);

            var uploaded = await dbx.Files.UploadAsync(
                                        "/" + file.FileName,
                                        WriteMode.Add.Instance,
                                        autorename: true,
                                        body: file.OpenReadStream());
            
            return uploaded != null;
        }
        // PUT: api/Mp3/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        [HttpGet("info")]
        public async Task<FileResult> GetDropboxMp3Async()
        {

            using var dbx = new DropboxClient(_conn);

            var file = await dbx.Files.DownloadAsync("/sample.mp3");

            var bytes = await file.GetContentAsByteArrayAsync();

            return File(bytes, "audio/mpeg", "dropbox.mp3");
        }
        [HttpGet("all")]
        public async Task<IList<string>> GetAllAsync()
        {
            using var dbx = new DropboxClient(_conn);

            var files = await dbx.Files.ListFolderAsync(string.Empty);

            List<string> names = new List<string>();

            foreach (var file in files.Entries)
            {
                names.Add(file.Name);
            }
         
            return names;
        }
    }
}
