using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnlightDenBackendAPI.Entities.Configurations;

public class ModuleConfig : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.ToTable("Modules", "General"); // Specify the table name and schema

        builder.HasKey(mod => mod.Id); // Define the primary key

        builder.Property(mod => mod.Id).IsRequired(); // Define required property of the primary key

        builder
            .HasOne(mod => mod.User)
            .WithMany()
            .HasForeignKey(mod => mod.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(mod => mod.Class)
            .WithMany()
            .HasForeignKey(mod => mod.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(mod => mod.Note)
            .WithMany()
            .HasForeignKey(mod => mod.NoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany<ModuleSubtopic>(s => s.Subtopics)
            .WithOne(mod => mod.Module)
            .HasForeignKey(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}