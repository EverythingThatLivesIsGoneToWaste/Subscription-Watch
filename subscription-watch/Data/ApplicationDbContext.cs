using Microsoft.EntityFrameworkCore;

namespace subscription_watch.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // TODO: add models and define DbSet properties

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TODO: configure data models in OnModelCreating
        }
    }
}
