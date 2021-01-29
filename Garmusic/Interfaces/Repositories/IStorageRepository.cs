using Garmusic.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Repositories
{
    public interface IStorageRepository
    {
        Task<IEnumerable<Storage>> GetAllAsync();
    }
}
