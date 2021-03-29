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
        public async Task DropboxWebhookMigrationAsync(IEnumerable<string> storageAccountsIDs)
        {
            await _migRepo.DropboxWebhookMigrationAsync(storageAccountsIDs);
        }

        public async Task DropboxMigrationAsync(int accountId)
        {
            await _migRepo.DropboxMigrationAsync(accountId);
        }

        public async Task GoogleDriveMigrationAsync(int accountId)
        {
            await _migRepo.GoogleDriveMigrationAsync(accountId);
        }

        public async Task GoogleDriveWebhookMigrationAsync(string channelID)
        {
            await _migRepo.GoogleDriveWebhookMigrationAsync(channelID);
        }

        public async Task RegisterOrRefreshGoogleDriveWebhook(int accountID)
        {
            await _migRepo.RegisterOrRefreshGoogleDriveWebhook(accountID);
        }
    }
}
