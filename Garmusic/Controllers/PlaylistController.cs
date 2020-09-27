using Microsoft.AspNetCore.Mvc;
using Garmusic.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Garmusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        // GET: api/<PlaylistController>
        [HttpGet]
        public Playlist GetPlaylist()
        {
            Playlist playlist = new Playlist();
            playlist.Add("Song 1");
            playlist.Add("Song 2");
            playlist.Add("Song 3");

            return playlist;
            //return new string[] { "value1", "value2" };
        }

        // GET api/<PlaylistController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PlaylistController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PlaylistController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PlaylistController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
