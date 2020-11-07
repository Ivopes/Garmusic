using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Models.Entities
{
    public class NotificationRequest
    {
        public ListFolder list_folder;
        public Delta delta;
        public class ListFolder
        {
            public ICollection<string> accounts;
        }
        public class Delta
        {
            public ICollection<int> users;
        }
    }

}
