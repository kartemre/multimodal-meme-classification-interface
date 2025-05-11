using Entities.Enums;

namespace Entities.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedTime{ get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
        public DataStatus Status { get; set; }
        public bool IsActive { get; set; }

        public BaseEntity()
        {
            CreatedTime = DateTime.Now;
            Status = DataStatus.Inserted;
            IsActive = true;
        }
    }
}
