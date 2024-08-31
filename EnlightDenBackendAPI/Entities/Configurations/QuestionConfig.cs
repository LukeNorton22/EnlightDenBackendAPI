using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnlightDenBackendAPI.Entities.Configurations
{
    public class QuestionConfig : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions", "General"); 

            builder.HasKey(q => q.Id);

            builder.Property(q => q.QuestionType)
                .HasConversion<int>() 
                .IsRequired(); 

            builder.HasOne(q => q.Class)
                .WithMany()  
                .HasForeignKey(q => q.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
