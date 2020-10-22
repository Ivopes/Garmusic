using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garmusic.Models
{
    [Table("playlist")]
    public class Playlist
    {
        public int PlaylistID { get; set; }
        public string Name { get; set; }
        public int AccountID { get; set; }
        public ICollection<PlaylistSong> Songs { get; set; }
    }
}
