using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnlightDenBackendAPI.Entities.Configurations;

public class MindMapTopicConfig : IEntityTypeConfiguration<MindMapTopic>
{
    public void Configure(EntityTypeBuilder<MindMapTopic> builder)
    {
        builder.ToTable("MindMapTopics", "General"); 

        builder.HasKey(mt => mt.Id);

        builder.Property(mt => mt.Name)
            .IsRequired()
            .HasMaxLength(100); 

   
    }
}
