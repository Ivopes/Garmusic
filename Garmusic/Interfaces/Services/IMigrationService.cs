using Garmusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Services
{
    public interface IMigrationService
    {
        Task DropboxMigrateAsync(IEnumerable<string> storageAccountsIDs);
    }
}
