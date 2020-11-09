using Garmusic.Interfaces.Repositories;
using Garmusic.Interfaces.Services;
using Garmusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Services.Dependencies
{
    public class MigrationService : IMigrationService
    {
        private readonly IMigrationRepository _migRepo;
        public MigrationService(IMigrationRepository migrationRepository)
        {
            _migRepo = migrationRepository;
        }
        public async Task DropboxMigrateAsync(IEnumerable<string> storageAccountsIDs)
        {
            await _migRepo.DropboxMigrationAsync(storageAccountsIDs);
        }
    }
}
