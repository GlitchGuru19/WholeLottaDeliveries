# 🚀 .NET 9 Key Concepts & Features Used

This guide explains the .NET 9 features and patterns used in the Delivery App.

## Table of Contents
1. [.NET 9 Overview](#net-9-overview)
2. [Blazor Server in .NET 9](#blazor-server-in-net-9)
3. [Entity Framework Core 9](#entity-framework-core-9)
4. [ASP.NET Core Identity](#aspnet-core-identity)
5. [SignalR](#signalr)
6. [Important C# Features](#important-c-features)

---

## .NET 9 Overview

### What's New in .NET 9?
- **Performance improvements**: Faster startup, lower memory usage
- **Enhanced Blazor**: Better server-side rendering
- **EF Core improvements**: Better SQLite support, performance gains
- **Better diagnostics**: Improved logging and error messages

### Why .NET 9?
- Latest features and security updates
- Best performance of any .NET version
- Long-term support (LTS) coming soon
- Modern C# language features (C# 13)

---

## Blazor Server in .NET 9

### What is Blazor Server?

Blazor Server is a framework for building interactive web UIs using C# instead of JavaScript.

**How it works:**
```
Browser ←→ SignalR WebSocket ←→ .NET Server
                                      ↓
                                 C# Code Executes
                                      ↓
                                  Database
```

### Key Concepts

#### 1. **Components (.razor files)**

Components are reusable UI pieces written in Razor syntax.

```razor
@* This is a Razor component *@
@page "/dashboard"              @* URL route *@

<h1>My Dashboard</h1>           @* HTML markup *@

@if (orders != null)            @* C# logic *@
{
    @foreach (var order in orders)  @* C# loop *@
    {
        <div>@order.Description</div>
    }
}

@code {                         @* C# code block *@
    private List<Order>? orders;

    protected override async Task OnInitializedAsync()
    {
        orders = await LoadOrders();
    }
}
```

#### 2. **Component Parameters**

Pass data between components:

```razor
@* Parent Component *@
<OrderCard Order="@currentOrder" />

@* Child Component *@
@code {
    [Parameter]
    public Order Order { get; set; } = null!;
}
```

#### 3. **Dependency Injection**

Inject services into components:

```razor
@inject ApplicationDbContext DbContext
@inject NavigationManager Navigation
@inject ILogger<MyComponent> Logger

@code {
    // Services are automatically available here
    await DbContext.SaveChangesAsync();
    Navigation.NavigateTo("/home");
    Logger.LogInformation("Something happened");
}
```

#### 4. **Event Handling**

Handle user interactions:

```razor
<button @onclick="HandleClick">Click Me</button>
<input @bind="userName" />  @* Two-way binding *@

@code {
    private string userName = "";

    private void HandleClick()
    {
        // Handle button click
    }
}
```

#### 5. **Form Handling**

Create forms with validation:

```razor
<EditForm Model="model" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />  @* Enable validation *@
    
    <InputText @bind-Value="model.Name" />
    <ValidationMessage For="@(() => model.Name)" />
    
    <button type="submit">Submit</button>
</EditForm>

@code {
    private MyModel model = new();
    
    private async Task HandleSubmit()
    {
        // Form is valid - save data
        await SaveData();
    }
}
```

---

## Entity Framework Core 9

### What is EF Core?

EF Core is an Object-Relational Mapper (ORM) that lets you work with databases using C# objects.

### Key Concepts

#### 1. **DbContext**

The main class for database operations:

```csharp
public class ApplicationDbContext : DbContext
{
    // Tables in your database
    public DbSet<Order> Orders { get; set; }
    public DbSet<ApplicationUser> Users { get; set; }
    
    // Configure relationships
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders);
    }
}
```

#### 2. **Entities (Models)**

C# classes that represent database tables:

```csharp
public class Order
{
    public int Id { get; set; }              // Primary Key
    public string UserId { get; set; }       // Foreign Key
    public ApplicationUser User { get; set; } // Navigation Property
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

#### 3. **LINQ Queries**

Query database using C# LINQ:

```csharp
// Get all orders for a user
var orders = await DbContext.Orders
    .Where(o => o.UserId == userId)
    .OrderByDescending(o => o.CreatedAt)
    .ToListAsync();

// Get single order
var order = await DbContext.Orders
    .FirstOrDefaultAsync(o => o.Id == orderId);

// Include related data (JOIN)
var ordersWithUsers = await DbContext.Orders
    .Include(o => o.User)
    .ToListAsync();
```

#### 4. **CRUD Operations**

**Create:**
```csharp
var order = new Order { Description = "..." };
DbContext.Orders.Add(order);
await DbContext.SaveChangesAsync();
```

**Read:**
```csharp
var order = await DbContext.Orders.FindAsync(id);
```

**Update:**
```csharp
order.Status = "Completed";
await DbContext.SaveChangesAsync();
```

**Delete:**
```csharp
DbContext.Orders.Remove(order);
await DbContext.SaveChangesAsync();
```

#### 5. **Migrations**

Track database schema changes:

```bash
# Create a migration
dotnet ef migrations add AddOrderTable

# Apply migrations to database
dotnet ef database update

# Revert last migration
dotnet ef migrations remove
```

---

## ASP.NET Core Identity

### What is Identity?

Identity is a complete authentication and authorization system.

### Key Concepts

#### 1. **User Management**

```csharp
// Create user
var user = new ApplicationUser { Email = "user@example.com" };
await UserManager.CreateAsync(user, "password");

// Find user
var user = await UserManager.FindByEmailAsync("user@example.com");

// Check password
var result = await UserManager.CheckPasswordAsync(user, "password");

// Add to role
await UserManager.AddToRoleAsync(user, "Admin");
```

#### 2. **Sign In Management**

```csharp
// Sign in
await SignInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

// Sign out
await SignInManager.SignOutAsync();

// Check if signed in
var isSignedIn = SignInManager.IsSignedIn(User);
```

#### 3. **Role Management**

```csharp
// Create role
await RoleManager.CreateAsync(new IdentityRole("Admin"));

// Check if role exists
var exists = await RoleManager.RoleExistsAsync("Admin");

// Check if user is in role
var isAdmin = await UserManager.IsInRoleAsync(user, "Admin");
```

#### 4. **Authorization in Razor**

```razor
@* Page level - entire page requires authentication *@
@attribute [Authorize]
@attribute [Authorize(Roles = "Admin")]

@* Component level - conditional rendering *@
<AuthorizeView>
    <Authorized>
        <p>Hello, @context.User.Identity.Name!</p>
    </Authorized>
    <NotAuthorized>
        <p>Please log in</p>
    </NotAuthorized>
</AuthorizeView>

<AuthorizeView Roles="Admin">
    <Authorized>
        <p>Admin only content</p>
    </Authorized>
</AuthorizeView>
```

---

## SignalR

### What is SignalR?

SignalR enables real-time, bidirectional communication between server and clients.

### Key Concepts

#### 1. **Hub (Server-side)**

```csharp
public class OrderHub : Hub
{
    // Send to all clients
    public async Task BroadcastMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
    
    // Send to specific client
    public async Task SendToUser(string userId, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveMessage", message);
    }
    
    // Send to caller only
    public async Task Echo(string message)
    {
        await Clients.Caller.SendAsync("ReceiveMessage", message);
    }
}
```

#### 2. **Client Connection (Blazor)**

```csharp
@code {
    private HubConnection? hubConnection;
    
    protected override async Task OnInitializedAsync()
    {
        // Create connection
        hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/orderhub"))
            .Build();
        
        // Listen for messages
        hubConnection.On<string>("ReceiveMessage", (message) =>
        {
            // Handle received message
            Console.WriteLine(message);
            StateHasChanged();  // Update UI
        });
        
        // Start connection
        await hubConnection.StartAsync();
    }
    
    // Send message to server
    private async Task SendMessage()
    {
        await hubConnection.SendAsync("BroadcastMessage", "Hello!");
    }
    
    // Clean up
    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}
```

---

## Important C# Features

### 1. **Async/Await**

Asynchronous programming for I/O operations:

```csharp
// Bad - blocks thread
var data = GetDataFromDatabase();

// Good - non-blocking
var data = await GetDataFromDatabaseAsync();

// Always use async for:
// - Database operations
// - HTTP requests
// - File I/O
// - SignalR calls
```

### 2. **Null Safety (C# 13)**

Prevent null reference exceptions:

```csharp
// Nullable reference types
string? nullableString = null;   // Can be null
string nonNullString = "Hello";  // Cannot be null

// Null-conditional operator
var length = text?.Length;  // null if text is null

// Null-coalescing operator
var name = user?.Name ?? "Guest";  // "Guest" if null

// Null-forgiving operator (use carefully!)
var length = text!.Length;  // "I promise text is not null"
```

### 3. **Pattern Matching**

```csharp
// Type patterns
if (obj is Order order)
{
    Console.WriteLine(order.Description);
}

// Property patterns
if (order is { Status: "Pending", Location: "Town" })
{
    // Order is pending and location is Town
}

// Switch expressions
var message = status switch
{
    "Pending" => "Waiting for admin",
    "In Progress" => "Being processed",
    "Completed" => "Done!",
    _ => "Unknown status"
};
```

### 4. **Collection Expressions (C# 13)**

```csharp
// Old way
var list = new List<string> { "a", "b", "c" };

// New way (C# 13)
List<string> list = ["a", "b", "c"];

// Array
string[] array = ["a", "b", "c"];

// Spread operator
int[] numbers = [1, 2, ..otherNumbers, 5];
```

### 5. **Primary Constructors (C# 13)**

```csharp
// Old way
public class MyService
{
    private readonly ILogger _logger;
    
    public MyService(ILogger logger)
    {
        _logger = logger;
    }
}

// New way (C# 13)
public class MyService(ILogger logger)
{
    public void DoSomething()
    {
        logger.LogInformation("Doing something");
    }
}
```

### 6. **Record Types**

Immutable data classes:

```csharp
// Record for DTOs
public record OrderDto(int Id, string Description, string Status);

// Use with deconstruction
var (id, desc, status) = orderDto;

// With expressions (create modified copy)
var updatedOrder = orderDto with { Status = "Completed" };
```

---

## Best Practices for .NET 9

### 1. **Always Use Async/Await**
```csharp
// ✅ Good
await DbContext.SaveChangesAsync();

// ❌ Bad
DbContext.SaveChanges();  // Blocks thread
```

### 2. **Dispose Resources Properly**
```csharp
// ✅ Good
public async ValueTask DisposeAsync()
{
    await hubConnection.DisposeAsync();
}

// Use 'using' statements
using var stream = File.OpenRead("file.txt");
```

### 3. **Use Dependency Injection**
```csharp
// ✅ Good - injected
@inject ApplicationDbContext DbContext

// ❌ Bad - direct instantiation
var context = new ApplicationDbContext();
```

### 4. **Handle Errors Gracefully**
```csharp
try
{
    await SaveData();
    successMessage = "Saved successfully!";
}
catch (Exception ex)
{
    logger.LogError(ex, "Error saving data");
    errorMessage = "Failed to save. Please try again.";
}
```

### 5. **Use Strong Typing**
```csharp
// ✅ Good
List<Order> orders = await GetOrdersAsync();

// ❌ Bad
var orders = await GetOrdersAsync();  // Type not immediately clear
```

---

## Common Mistakes to Avoid

### 1. **Forgetting to Call SaveChangesAsync()**
```csharp
// ❌ Bad - changes not saved!
order.Status = "Completed";

// ✅ Good
order.Status = "Completed";
await DbContext.SaveChangesAsync();
```

### 2. **Not Disposing SignalR Connections**
```csharp
// ❌ Bad - memory leak!
hubConnection = new HubConnectionBuilder().Build();

// ✅ Good - implement IAsyncDisposable
@implements IAsyncDisposable
```

### 3. **Blocking on Async Code**
```csharp
// ❌ Bad - can cause deadlocks!
var result = SomeAsyncMethod().Result;

// ✅ Good
var result = await SomeAsyncMethod();
```

### 4. **Not Using ConfigureAwait in Libraries**
```csharp
// For library code
await DoSomething().ConfigureAwait(false);
```

---

## Learning Resources

### Official Documentation
- [.NET 9 Release Notes](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-9)
- [C# 13 Features](https://learn.microsoft.com/dotnet/csharp/whats-new/csharp-13)
- [Blazor Tutorials](https://learn.microsoft.com/aspnet/core/blazor/tutorials/)
- [EF Core Documentation](https://learn.microsoft.com/ef/core/)

### Video Tutorials
- [.NET YouTube Channel](https://youtube.com/dotnet)
- [Microsoft Learn](https://learn.microsoft.com/training/browse/?products=dotnet)

### Community
- [Stack Overflow](https://stackoverflow.com/questions/tagged/.net)
- [.NET Discord](https://aka.ms/dotnet-discord)
- [Reddit r/dotnet](https://reddit.com/r/dotnet)

---

**Happy Coding with .NET 9! 🚀**
