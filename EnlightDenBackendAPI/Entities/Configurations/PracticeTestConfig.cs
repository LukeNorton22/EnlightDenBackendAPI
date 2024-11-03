using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnlightDenBackendAPI.Entities.Configurations
{
    public class PracticeTestConfig : IEntityTypeConfiguration<PracticeTest>
    {
        public void Configure(EntityTypeBuilder<PracticeTest> builder)
        {
            builder.ToTable("PracticeTest", "General");
            builder.HasKey(pt => pt.Id);

            builder
                .HasOne(pt => pt.StudyModule)
                .WithMany(sm => sm.PracticeTests)
                .HasForeignKey(pt => pt.StudyModuleId);
        }
    }
}

