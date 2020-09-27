using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Garmusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Mp4Controller : ControllerBase
    {
        // GET: api/Mp4
        [HttpGet]
        public FileResult GetMp4(int id)
        {
            byte[] bytes;
            string fileLocation = "Assets/sample.mp4";
            using (var reader = new BinaryReader(new FileStream(fileLocation, FileMode.Open)))
            {
                long bytesNum = new FileInfo(fileLocation).Length;
                bytes = reader.ReadBytes((int)bytesNum);
            }
            return File(bytes, "video/mp4", "stahnuty.mp4");
        }
        // GET api/<Mp4Controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<Mp4Controller>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Mp4Controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Mp4Controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
