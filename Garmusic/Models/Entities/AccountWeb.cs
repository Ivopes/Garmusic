using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Models.Entities
{
    /// <summary>
    /// Class representing data to send to frontEnd - no pass, etc...
    /// </summary>
    public class AccountWeb
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public IList<Storage> Storage { get; set; }
    }
}
