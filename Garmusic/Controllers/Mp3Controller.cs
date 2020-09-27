using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Garmusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Mp3Controller : ControllerBase
    {       
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
        public void Post([FromBody] string value)
        {
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
    }
}
