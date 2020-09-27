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
        public MusicPlayerContext(DbContextOptions<MusicPlayerContext> options) : base(options)
        {

        }
        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=MusicPlayer;Trusted_Connection=True;");
        }*/
    }
}
