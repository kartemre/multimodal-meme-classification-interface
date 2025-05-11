using Entities.Enums;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MAP.Options
{
    public abstract class BaseMap<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatedTime)
                   .IsRequired(); 

            builder.Property(x => x.UpdatedTime)
                   .IsRequired(false);

            builder.Property(x => x.DeletedTime)
                   .IsRequired(false);

            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasConversion(
                       v => v.ToString(),
                       v => (DataStatus)Enum.Parse(typeof(DataStatus), v)
                   );

            builder.Property(x => x.IsActive)
                   .IsRequired();
        }
    }
}
