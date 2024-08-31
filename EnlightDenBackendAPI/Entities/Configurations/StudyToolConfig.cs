using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnlightDenBackendAPI.Entities.Configurations;

public class StudyToolConfig : IEntityTypeConfiguration<StudyTool>
{
    public void Configure(EntityTypeBuilder<StudyTool> builder)
    {
        builder.ToTable("StudyTools", "General");

        builder.HasKey(st => st.Id);

        builder.Property(st => st.Id)
            .IsRequired();

        builder.HasOne(st => st.User)
            .WithMany()  
            .HasForeignKey(st => st.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(st => st.Class)
            .WithMany()  
            .HasForeignKey(st => st.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(st => st.MindMapTopic)
            .WithMany()  
            .HasForeignKey(st => st.MindMapTopicId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(st => st.ContentType)
            .HasConversion<int>() 
            .IsRequired();
      
    }
}
