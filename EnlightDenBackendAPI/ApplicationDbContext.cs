using EnlightDenBackendAPI.Entities;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    //Add your db sets here, each time a new entity is created
    public DbSet<User> Users { get; set; }
    public DbSet<Class> Classes { get; set; }

    //Apply the configurations each time they are created. 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new UserConfig());

    }
}
