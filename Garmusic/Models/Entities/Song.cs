using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Garmusic.Models
{
    [Table("song")]
    public class Song
    {
        public int SongID { get; set; }
        public string Name { get; set; }
        public int AccountID { get; set; }
        public Account Account { get; set; }

    }
}
