using EnlightDenBackendAPI.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class ClassConfig : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.ToTable("Class", "public"); // Specify the table name and schema
        builder.HasKey(c => c.Id); // Define the primary key

        // Configure UserId as a foreign key
        builder.HasOne(c => c.User)
            .WithMany() // Adjust this if a user can have multiple classes
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade); 

    }
}
