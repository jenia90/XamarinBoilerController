using System;
using System.Collections.Generic;
using System.Linq;
using BoilerController.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BoilerController.Api.Repository
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Device> Devices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>().Property(e => e.DaysList)
                .HasConversion(v => string.Join(",", v), v => Enumerate(v)); 
            base.OnModelCreating(modelBuilder);
        }

        private static IEnumerable<DayOfWeek> Enumerate(string v)
        {
            IEnumerable<DayOfWeek> val = new List<DayOfWeek>();
            foreach (var s in v.Split(','))
            {
                val.Append(Enum.Parse<DayOfWeek>(s));
            }

            return val;
        }
    }
}
