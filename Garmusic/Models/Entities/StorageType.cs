using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Models.Entities
{
    /// <summary>
    /// Enum for better readability. Values must match the database
    /// </summary>
    public enum StorageType
    {
        Dropbox = 1,
        GoogleDrive = 2
    }
}
