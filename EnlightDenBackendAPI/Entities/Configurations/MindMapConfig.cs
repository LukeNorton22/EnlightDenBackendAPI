using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnlightDenBackendAPI.Entities.Configurations
{
    public class MindMapConfig : IEntityTypeConfiguration<MindMap>
    {
        public void Configure(EntityTypeBuilder<MindMap> builder)
        {
            builder.ToTable("MindMaps", "General"); 

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .IsRequired();

            builder.HasOne(m => m.Class)                  
                .WithMany()                               
                .HasForeignKey(m => m.ClassId)            
                .OnDelete(DeleteBehavior.Cascade);        

            builder.HasOne(m => m.User)                   
                .WithMany()                               
                .HasForeignKey(m => m.UserId)             
                .OnDelete(DeleteBehavior.Cascade);        

            builder.HasMany<MindMapTopic>(m => m.Topics)  
                .WithOne(t => t.MindMap)                  
                .HasForeignKey(t => t.MindMapId)          
                .OnDelete(DeleteBehavior.Cascade);        
        }
    }
}
