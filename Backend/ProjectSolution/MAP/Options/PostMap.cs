using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MAP.Options
{
    public class PostMap : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Text)
                .IsRequired();

            builder.Property(x => x.ImageData)
                .IsRequired(false);

            builder.Property(x => x.UserId)
                .IsRequired();

            // User ile ilişki
            builder.HasOne(x => x.User)
                .WithMany()  // AppUser'da karşılık gelen collection property'si yoksa
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Kullanıcı silindiğinde postları silinmesin

        }
    }
} 