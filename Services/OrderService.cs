using DeliveryApp.Data;
using DeliveryApp.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WholeLottaDeliveries.Models;

namespace WholeLottaDeliveries.Services;

// Service responsible for handling order-related business logic
public class OrderService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHubContext<OrderHub> _hubContext;

    // Constructor for dependency injection of required services
    public OrderService(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        IHubContext<OrderHub> hubContext)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _hubContext = hubContext;
    }

    // Creates a new order for the specified user
    // Returns a tuple containing success status and an optional error message
    public async Task<(bool Success, string? ErrorMessage)> CreateOrderAsync(string userId, OrderModel orderModel)
    {
        try
        {
            // Verify that the user exists in the database
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found. Please log in again.");
            }

            // Create a new Order entity with the provided data
            var order = new Order
            {
                UserId = userId,
                Description = orderModel.Description,
                Location = orderModel.Location,
                EstimatedTime = orderModel.EstimatedTime,
                DeliveryInstructions = orderModel.DeliveryInstructions,
                Status = "Pending", // All new orders start with "Pending" status
                CreatedAt = DateTime.Now
            };

            // Add the order to the database context
            _dbContext.Orders.Add(order);
            
            // Persist the changes to the database
            await _dbContext.SaveChangesAsync();

            // Notify all connected SignalR clients about the new order
            // This allows real-time updates for delivery personnel and admins
            await _hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", "New order created");

            // Return success with no error message
            return (true, null);
        }
        catch (Exception ex)
        {
            // Log the exception and return a user-friendly error message
            // In production, you might want to use proper logging (ILogger)
            return (false, $"Error creating order: {ex.Message}");
        }
    }

    // Gets all orders for a specific user
    // Returns a list of orders belonging to the user
    public async Task<List<Order>> GetUserOrdersAsync(string userId)
    {
        // Retrieve all orders for the specified user, ordered by creation date (newest first)
        return await _dbContext.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    // Cancels an order if it's still in Pending status
    // Returns success status and optional error message
    public async Task<(bool Success, string? ErrorMessage)> CancelOrderAsync(int orderId, string userId)
    {
        try
        {
            // Find the order by ID
            var order = await _dbContext.Orders.FindAsync(orderId);

            // Check if order exists
            if (order == null)
            {
                return (false, "Order not found.");
            }

            // Verify that the order belongs to the user (security check)
            if (order.UserId != userId)
            {
                return (false, "You are not authorized to cancel this order.");
            }

            // Only allow cancellation if order is still pending
            if (order.Status != "Pending")
            {
                return (false, $"Cannot cancel order with status: {order.Status}");
            }

            // Update the order status to Cancelled
            order.Status = "Cancelled";
            await _dbContext.SaveChangesAsync();

            // Notify all connected SignalR clients about the cancellation
            await _hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", "Order cancelled");

            return (true, null);
        }
        catch (Exception ex)
        {
            // Return error message if something goes wrong
            return (false, $"Error cancelling order: {ex.Message}");
        }
    }
}
