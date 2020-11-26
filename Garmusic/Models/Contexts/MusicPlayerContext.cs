using Garmusic.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Garmusic.Models
{
    public class MusicPlayerContext: DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<AccountStorage> AccountStorages { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public MusicPlayerContext(DbContextOptions<MusicPlayerContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<AccountStorage>()
                .HasKey(ac => new { ac.AccountID, ac.StorageID });

        }
    }
}
