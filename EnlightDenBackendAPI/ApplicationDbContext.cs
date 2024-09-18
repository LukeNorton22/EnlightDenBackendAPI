using EnlightDenBackendAPI.Entities;
using EnlightDenBackendAPI.Entities.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    //Add your db sets here, each time a new entity is created
    public DbSet<User> Users { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<StudyPlan> StudyPlans { get; set; }
    public DbSet<MindMap> MindMaps { get; set; }
    public DbSet<MindMapTopic> MindMapTopics { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<StudyTool> StudyTools { get; set; }

    //Apply the configurations each time they are created.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new UserConfig());
        modelBuilder.ApplyConfiguration(new ClassConfig());
        modelBuilder.ApplyConfiguration(new NoteConfig());
        modelBuilder.ApplyConfiguration(new StudyPlanConfig());
        modelBuilder.ApplyConfiguration(new MindMapConfig());
        modelBuilder.ApplyConfiguration(new MindMapTopicConfig());
        modelBuilder.ApplyConfiguration(new QuestionConfig());
        modelBuilder.ApplyConfiguration(new StudyToolConfig());
    }
}
