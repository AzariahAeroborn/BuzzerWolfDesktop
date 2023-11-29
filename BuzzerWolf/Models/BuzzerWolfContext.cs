using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace BuzzerWolf.Models
{
    public class BuzzerWolfContext : DbContext
    {
        public DbSet<Profile> Profiles { get; set; }

        private string _dbPath;

        public BuzzerWolfContext()
        {
            _dbPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "buzzerwolf.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }
}
