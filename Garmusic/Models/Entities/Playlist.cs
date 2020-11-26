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
        public int Id { get; set; }
        public string Name { get; set; }
        public int AccountID { get; set; }
        public ICollection<Song> Songs { get; set; }
    }
}
