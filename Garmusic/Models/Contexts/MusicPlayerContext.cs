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
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public MusicPlayerContext(DbContextOptions<MusicPlayerContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PlaylistSong>()
                .HasKey(ps => new { ps.PlaylistID, ps.SongID });
        }
    }
}
