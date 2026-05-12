using Karakatsiya.Models.Entities.Common;
using Karakatsiya.Models.Enums;

namespace Karakatsiya.Models.Entities.Admin
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
