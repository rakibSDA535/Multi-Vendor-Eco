using Microsoft.AspNetCore.Identity;

namespace Shop111.Models
{
    public enum ManagerStatus { Pending, Approved, Rejected }
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public bool IsApproved { get; set; } = false;
        public virtual Vendor? Vendor { get; set; }
        public ManagerStatus Status { get; set; } = ManagerStatus.Pending;
    }
}
