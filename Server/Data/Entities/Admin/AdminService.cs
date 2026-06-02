using Karakatsiya.Data.Entities.Common;
using Karakatsiya.Data.Enums;

namespace Karakatsiya.Data.Entities.Admin
{
    public class AdminService : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public ServiceType Type { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
