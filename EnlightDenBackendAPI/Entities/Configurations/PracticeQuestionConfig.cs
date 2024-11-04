using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnlightDenBackendAPI.Entities;

namespace EnlightDenBackendAPI.Entities.Configurations
{
    public class PracticeQuestionConfig : IEntityTypeConfiguration<PracticeQuestion>
    {
        public void Configure(EntityTypeBuilder<PracticeQuestion> builder)
        {
            builder.ToTable("PracticeQuestion", "General");
            builder.HasKey(pq => pq.Id);

            builder
                .HasOne(pq => pq.PracticeTest)
                .WithMany(pt => pt.PracticeQuestions)
                .HasForeignKey(pq => pq.PracticeTestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
