using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Models.Entities
{
    [Table("storage")]
    public class Storage
    {
        public int StorageID { get; set; }
        public string Name { get; set; }
    }
}
