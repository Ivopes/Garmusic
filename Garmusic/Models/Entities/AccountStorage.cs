using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Models.Entities
{
    [Table("account_storage")]
    public class AccountStorage
    {
        public int AccountID { get; set; }
        public Account Account { get; set; }
        public int StorageID { get; set; }
        public Storage Storage { get; set; }
        public string JsonData { get; set; }
        public string StorageAccountID { get; set; }
    }
}
