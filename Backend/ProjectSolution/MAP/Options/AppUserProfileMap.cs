using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MAP.Options
{
    public class AppUserProfileMap : BaseMap<AppUserProfile>
    {
        public override void Configure(EntityTypeBuilder<AppUserProfile> builder)
        {
            // BaseMap'teki ortak özellikleri uygula
            base.Configure(builder);

            // AppUserProfile için özel yapılandırmalar
            builder.Property(x => x.FirstName)
                   .IsRequired()
                   .HasMaxLength(50); // İsim maksimum 50 karakter

            builder.Property(x => x.LastName)
                   .IsRequired()
                   .HasMaxLength(50); // Soyisim maksimum 50 karakter

            builder.Property(x => x.PhoneNumber)
                   .HasMaxLength(20); // Telefon numarası maksimum 20 karakter

            builder.Property(x => x.Mail)
                   .IsRequired()
                   .HasMaxLength(100); // E-posta maksimum 100 karakter

            builder.Property(x => x.Role)
                   .IsRequired(); // Kullanıcı rolü zorunlu
        }
    }
}
