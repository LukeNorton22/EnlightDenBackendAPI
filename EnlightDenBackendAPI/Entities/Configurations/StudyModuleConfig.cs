using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace EnlightDenBackendAPI.Entities.Configurations
{
    public class StudyModuleConfig : IEntityTypeConfiguration<StudyModule>
    {
        public void Configure(EntityTypeBuilder<StudyModule> builder)
        {
            builder.ToTable("StudyModule", "General");

            builder.HasKey(c => c.Id);

            builder
                .HasOne(sm =>  sm.StudyTool)
                .WithOne(st => st.StudyModule)
                .HasForeignKey<StudyModule>(sm => sm.StudyToolId)
                .OnDelete(DeleteBehavior.Cascade);
        }   
    }

}
