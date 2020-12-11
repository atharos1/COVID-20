using COVID_20.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COVID_20.Persistence {
    public class AppDbContext : DbContext {
        public DbSet<Case> Cases { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<DatabaseUpdate> DatabaseUpdates { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        }

    }
}
