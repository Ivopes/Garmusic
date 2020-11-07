using Dropbox.Api.Users;
using Garmusic.Models.Entities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ICollection<Song> Songs { get; set; }
        public IList<AccountStorage> AccountStorages { get; set; }
        [NotMapped]
        public string Password { get; set; }
        
    }
}
