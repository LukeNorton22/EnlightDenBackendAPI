using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnlightDenBackendAPI.Entities.Configurations
{
    public class StudyPlanConfig : IEntityTypeConfiguration<StudyPlan>
    {
        public void Configure(EntityTypeBuilder<StudyPlan> builder)
        {
            // Specify the table name and schema (if needed)
            builder.ToTable("StudyPlans", "General"); // Adjust schema as necessary

            // Define the primary key
            builder.HasKey(sp => sp.Id);

            // Define properties
            builder.Property(sp => sp.Name)
                .IsRequired();

            builder.Property(sp => sp.Description)
                .HasMaxLength(500); 

            builder.Property(sp => sp.Day)
                .IsRequired();

            builder.Property(sp => sp.Month)
                .IsRequired();

            builder.Property(sp => sp.StartTime)
                .IsRequired();

            builder.Property(sp => sp.EndTime)
                .IsRequired();

            builder.HasOne(sp => sp.User)           
                .WithMany()                         
                .HasForeignKey(sp => sp.UserId)     
                .OnDelete(DeleteBehavior.Cascade);  

        }
    }
}
