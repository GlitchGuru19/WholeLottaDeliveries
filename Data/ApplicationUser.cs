using Microsoft.AspNetCore.Identity;

namespace DeliveryApp.Data;

public class ApplicationUser : IdentityUser
{
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
