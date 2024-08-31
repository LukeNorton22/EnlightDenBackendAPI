// Configurations/UserConfiguration.cs
using EnlightDenBackendAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User", "Authorization"); // Specify the table name and schema
        builder.HasKey(u => u.Id); // Define the primary key

        // Additional configurations if needed
    }
}
