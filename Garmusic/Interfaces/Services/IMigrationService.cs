﻿using Garmusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Services
{
    public interface IMigrationService
    {
        Task DropboxWebhookMigrationAsync(IEnumerable<string> storageAccountsIDs);
        Task DropboxMigrationAsync(int accountId);
        Task GoogleDriveMigrationAsync(int accountId);
        Task GoogleDriveWebhookMigrationAsync(string channelID);
        Task RegisterOrRefreshGoogleDriveWebhook(int accountID);
    }
}
