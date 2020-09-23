﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API_BAK.Models
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
        [Column("password")]
        public string Password { get; set; }
        [Column("created")]
        public DateTime Created { get; set; }

        /*
        [Column("password_salt")]
        public byte[] PasswordSalt { get; set; }
        [NotMapped]
        public byte[] PasswordHash { get; set; }
        */
    }
}