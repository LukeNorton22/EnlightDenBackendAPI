using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Define your DbSets (tables) here
    // Example:
    // public DbSet<YourEntity> YourEntities { get; set; }
}
