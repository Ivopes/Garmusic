﻿using Garmusic.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Interfaces.Services
{
    public interface IStorageService
    {
        /// <summary>
        /// Get list of all Storages
        /// </summary>
        /// <returns>IEnumarable of all Storages</returns>
        Task<IEnumerable<Storage>> GetAllAsync();
    }
}
