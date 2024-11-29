using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace BuzzerWolf.Models
{
    public class BuzzerWolfContext : DbContext
    {
        public int LoggedInTeam { get; set; } = -1;

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<PlayoffSchedule> LeaguePlayoffs { get; set; }
        public DbSet<Result> LeagueResults { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Standings> Standings { get; set; }
        public DbSet<Sync> Sync { get; set; }

        private string _dbPath;

        public BuzzerWolfContext()
        {
            _dbPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "buzzerwolf.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite($"Data Source={_dbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
