using System.ComponentModel.DataAnnotations;

namespace WholeLottaDeliveries.Models;

// View model for creating a new order
// Contains validation attributes for form input
public class OrderModel
{
    // Description of what items need to be delivered
    // Must be between 5 and 1000 characters
    [Required(ErrorMessage = "Please describe what you need")]
    [StringLength(1000, MinimumLength = 5)]
    public string Description { get; set; } = string.Empty;

    // The location where the order should be delivered
    // Must be one of the predefined locations
    [Required(ErrorMessage = "Please select a delivery location")]
    public string Location { get; set; } = string.Empty;

    // The time by which the customer needs the items delivered
    // Format should be HH:mm (e.g., "14:30")
    [Required(ErrorMessage = "Please enter the time you need the items")]
    public string EstimatedTime { get; set; } = string.Empty;

    // Optional delivery instructions for special requests
    // Examples: "Call when you arrive", "Room 2, Block A"
    [StringLength(500)]
    public string? DeliveryInstructions { get; set; }

    // Calculate delivery fee based on selected location
    // *** TO CHANGE PRICES: Simply update the amounts in the switch statement below ***
    public decimal GetDeliveryFee()
    {
        return Location switch
        {
            "Town" => 30.00m,           // CHANGE THIS: Town delivery fee (K10)
            "VML Market" => 20.00m,      // CHANGE THIS: VML Market delivery fee (K8)
            "Campus" => 15.00m,          // CHANGE THIS: Campus delivery fee (K5)
            "Mukuba Mall" => 30.00m,    // CHANGE THIS: Mukuba Mall delivery fee (K12)
            _ => 0.00m                  // Default: No fee if location not selected
        };
    }
}
