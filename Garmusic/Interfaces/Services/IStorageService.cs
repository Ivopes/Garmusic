using Garmusic.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Services
{
    public interface IStorageService
    {
        Task<IEnumerable<Storage>> GetAllAsync();
    }
}
