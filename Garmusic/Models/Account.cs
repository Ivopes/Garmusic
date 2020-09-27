using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garmusic.Models
{
    [Table("account")]
    public class Account
    {
        [Column("account_id")]
        public long Id { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("password_hash")]
        public byte[] PasswordHash { get; set; }
        [Column("password_salt")]
        public byte[] PasswordSalt { get; set; }
        [Column("created")]
        public DateTime Created { get; set; }
        [NotMapped]
        public string Password { get; set; }
        
    }
}
