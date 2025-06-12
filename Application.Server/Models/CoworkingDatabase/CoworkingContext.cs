using Microsoft.EntityFrameworkCore;

namespace Application.Server.Models.CoworkingDatabase
{
    public class CoworkingContext : DbContext
    {
        public CoworkingContext(DbContextOptions<CoworkingContext> options) : base(options) { }
        public DbSet<Coworking> Coworkings { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var eTypes = modelBuilder.Model.GetEntityTypes();
            foreach (var type in eTypes)
            {
                var foreignKeys = type.GetForeignKeys();
                foreach (var foreignKey in foreignKeys)
                {
                    if (foreignKey.IsRequired)
                    {
                        foreignKey.DeleteBehavior = DeleteBehavior.ClientCascade;
                    }
                    else
                    {
                        foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
                    }
                }
            }
        }
    }
}
