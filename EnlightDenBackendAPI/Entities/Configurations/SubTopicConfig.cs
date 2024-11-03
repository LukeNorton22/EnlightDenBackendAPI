using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnlightDenBackendAPI.Entities.Configurations
{
    public class SubTopicConfig : IEntityTypeConfiguration<SubTopic>
    {
        public void Configure(EntityTypeBuilder<SubTopic> builder)
        {
            builder.ToTable("SubTopic", "General");
            builder.HasKey(c => c.Id);

            builder
                .HasOne(st => st.StudyModule)
                .WithMany(sm => sm.SubTopics)
                .HasForeignKey(st => st.StudyModuleId);
        }
    }
}
