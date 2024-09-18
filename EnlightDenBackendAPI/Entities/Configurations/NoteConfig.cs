using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnlightDenBackendAPI.Entities.Configurations
{
    public class NoteConfig : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.ToTable("Notes", "General");

            // Define the primary key
            builder.HasKey(n => n.Id);

            // Define properties
            builder.Property(n => n.Title).IsRequired();

            // Relationship with User
            builder
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Class
            builder
                .HasOne(n => n.Class)
                .WithMany()
                .HasForeignKey(n => n.ClassId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
