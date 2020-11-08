using Dropbox.Api;
using Garmusic.Interfaces.Repositories;
using Garmusic.Models;
using Garmusic.Models.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Repositories
{
    public class MigrationRepository : IMigrationRepository
    {

        public async Task DropboxMigrationAsync(IEnumerable<Account> accounts)
        {
            foreach (var acc in accounts)
            {
                StorageJson json = JsonConvert.DeserializeObject<StorageJson>(acc.AccountStorages[0].JsonData);



                var dbx = new DropboxClient("sl.AlKX0_pF1dAb8JmBSHf8pF4LVEZQKn8UZd4-DLhhNrrVNkNrTUSRWixEL4MmVdpLSUHlf0R2Uwsc9Sq1Hm92bjeIdRlayUKvZ8oB1mXZ58IFMakFNFD8mfw6xZOggGAmTc3Y_jo");

                var files = await dbx.Files.ListFolderContinueAsync(json.Cursor);


            }
            throw new NotImplementedException();
        }
    }
}
