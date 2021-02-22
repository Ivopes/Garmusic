using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Models.EntitiesWeb
{
    public class SongWeb
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public int LengthSec { get; set; }
        public int StorageID { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
    }
}
