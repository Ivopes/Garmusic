using Garmusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Repositories
{
    public interface IMigrationRepository
    {
        Task DropboxMigrationAsync(IEnumerable<string> storageAccountsIDs);
    }
}
