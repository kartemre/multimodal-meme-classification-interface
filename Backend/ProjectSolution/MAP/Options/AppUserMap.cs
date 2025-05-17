using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MAP.Options
{
    public class AppUserMap : BaseMap<AppUser>
    {
        public override void Configure(EntityTypeBuilder<AppUser> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.UserName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Password)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.PreviousPassword)
                   .HasMaxLength(255);

            builder.Property(x => x.PasswordResetToken)
                   .HasMaxLength(100);

            builder.Property(x => x.PasswordResetTokenExpiry);

            builder.HasOne(x => x.Profile)
                   .WithOne(x => x.User)
                   .HasForeignKey<AppUserProfile>(x => x.Id)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
