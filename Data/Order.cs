using System.ComponentModel.DataAnnotations;

namespace DeliveryApp.Data;

public class Order
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    public virtual ApplicationUser? User { get; set; }
    
    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public string Location { get; set; } = string.Empty;
    
    // Time when delivery is needed (e.g., "2:30 PM", "14:30")
    [Required]
    public string EstimatedTime { get; set; } = string.Empty;
    
    // Order status: "Pending", "In Progress", "Completed"
    [Required]
    public string Status { get; set; } = "Pending";
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
