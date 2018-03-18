using BoilerController.Api.Contracts;
using BoilerController.Api.Devices;
using BoilerController.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BoilerController.Api.Repository
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<IDevice> Devices { get; set; }
    }
}
