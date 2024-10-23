using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnlightDenBackendAPI.Entities.Configurations;

public class PracticeTestConfig : IEntityTypeConfiguration<PracticeTest>
{
    public void Configure(EntityTypeBuilder<PracticeTest> builder)
    {
        builder.ToTable("PracticeTests", "General"); // Specify the table name and schema

        builder.HasKey(p => p.Id); // Define the primary key

        builder.Property(p => p.Id).IsRequired(); // Define required property of the primary key

        builder
            .HasOne(p => p.Module)
            .WithMany()
            .HasForeignKey(p => p.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(p => p.Questions)
            .WithOne(q => q.PracticeTest)
            .HasForeignKey(q => q.PracticeTestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
