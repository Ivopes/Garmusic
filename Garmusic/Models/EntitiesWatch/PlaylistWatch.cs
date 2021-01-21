using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Models.EntitiesWatch
{
    public class PlaylistWatch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Sync { get; set; }
        public ICollection<int> SongsIds { get; set; }
    }
}
