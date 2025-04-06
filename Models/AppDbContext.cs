using Microsoft.EntityFrameworkCore;

namespace GCRAS.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet <User> Users { get; set; }
        public DbSet<Source> Sources { get; set; }
    }
}