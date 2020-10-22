using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garmusic.Models
{
    [Table("account")]
    public class Account
    {
        public int AccountID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime Created { get; set; }
        //public string DropboxCursor { get; set; }
        public ICollection<Song> Songs { get; set; }
        [NotMapped]
        public string Password { get; set; }
        
    }
}
