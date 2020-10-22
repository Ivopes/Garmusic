using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Models
{
    [Table("playlist_song")]
    public class PlaylistSong
    {
        public int PlaylistID { get; set; }
        public Playlist Playlist { get; set; }
        public int SongID { get; set; }
        public Song Song { get; set; }
    }
}
