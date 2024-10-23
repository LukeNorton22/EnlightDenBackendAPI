using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnlightDenBackendAPI.Entities.Configurations;

public class ModuleSubtopicConfig : IEntityTypeConfiguration<ModuleSubtopic>
{
    public void Configure(EntityTypeBuilder<ModuleSubtopic> builder)
    {
        builder.ToTable("ModuleSubtopics", "General");

        builder.HasKey(ms => ms.Id);

        builder.Property(ms => ms.Name).IsRequired().HasMaxLength(100);

        builder
            .HasOne<Module>(ms => ms.Module)
            .WithMany(ms => ms.Subtopics)
            .HasForeignKey(ms => ms.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

