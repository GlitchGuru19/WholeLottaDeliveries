# 🏗️ Architecture Documentation - .NET 9 Delivery App

## Overview

This document explains the architecture and key design decisions for the Delivery App built with .NET 9 Blazor Server.

## Technology Stack

### .NET 9 Features Used
- **Blazor Server**: Interactive server-side rendering for real-time UI updates
- **Entity Framework Core 9.0**: ORM for database operations
- **ASP.NET Core Identity 9.0**: Authentication and authorization
- **SignalR**: Real-time bidirectional communication
- **SQLite**: Lightweight file-based database

## Project Structure

```
DeliveryApp/
│
├── Components/                          # Blazor UI Components
│   ├── Layout/
│   │   └── MainLayout.razor            # Main layout with navigation
│   ├── Pages/                          # Page components (routes)
│   │   ├── Home.razor                  # Landing page (/)
│   │   ├── Login.razor                 # Login page (/login)
│   │   ├── Register.razor              # Registration (/register)
│   │   ├── CreateOrder.razor           # Create order (/create-order)
│   │   ├── UserDashboard.razor         # User orders (/dashboard)
│   │   └── AdminDashboard.razor        # Admin panel (/admin)
│   ├── App.razor                       # Root component
│   ├── Routes.razor                    # Routing configuration
│   ├── _Imports.razor                  # Global using statements
│   ├── RedirectToLogin.razor           # Auth redirect helper
│   └── IdentityRevalidatingAuthenticationStateProvider.cs
│
├── Data/                               # Data Models & Database
│   ├── ApplicationUser.cs              # User entity (extends IdentityUser)
│   ├── Order.cs                        # Order entity
│   └── ApplicationDbContext.cs         # EF Core DbContext
│
├── Hubs/                               # SignalR Hubs
│   └── OrderHub.cs                     # Real-time order updates
│
├── Migrations/                         # EF Core Migrations
│   └── [Generated migration files]
│
├── wwwroot/                            # Static files
│   └── app.css                         # Application styles
│
├── Program.cs                          # Application entry point
├── appsettings.json                    # Configuration
└── DeliveryApp.csproj                  # Project file
```

## Architecture Patterns

### 1. **Blazor Server Architecture**

**How it works:**
- Client opens a WebSocket connection to the server
- UI interactions trigger server-side C# code
- Server sends UI updates back to the client via SignalR
- All application logic runs on the server

**Benefits:**
- Full .NET API available (no JavaScript limitations)
- Smaller initial download (no WebAssembly runtime)
- Better for apps with sensitive business logic
- Direct database access from components

**Trade-offs:**
- Requires persistent connection to server
- Server memory used per connected client
- Not suitable for offline scenarios

### 2. **Entity Framework Core with SQLite**

**Data Model:**
```
ApplicationUser (IdentityUser)
    ├── Id (PK)
    ├── Email
    ├── PasswordHash
    └── Orders (Navigation Property)
            ↓
Order
    ├── Id (PK)
    ├── UserId (FK) → ApplicationUser
    ├── Description
    ├── Location
    ├── EstimatedTime
    ├── Status
    └── CreatedAt
```

**Relationship:**
- One-to-Many: One User → Many Orders
- Cascade Delete: Deleting a user deletes all their orders
- Lazy Loading: User navigation property loaded on-demand

### 3. **ASP.NET Core Identity**

**Security Features:**
- Password hashing using PBKDF2 algorithm
- Security stamps for session invalidation
- Role-based authorization (Admin/User)
- Anti-forgery token protection

**Roles:**
- **Admin**: Can view all orders, change order status
- **User**: Can create orders, view own orders only

### 4. **SignalR Real-time Updates**

**How it works:**
```
User creates order
    ↓
Server saves to database
    ↓
Server broadcasts via OrderHub
    ↓
All connected clients receive update
    ↓
Components refresh automatically
```

**Implementation:**
- Hub: `OrderHub.cs` - Server-side hub
- Client: JavaScript SignalR client library
- Connection: WebSocket (falls back to Server-Sent Events or Long Polling)

## Component Lifecycle

### Page Components (.razor files)

**Common Lifecycle Methods:**

```csharp
@code {
    // 1. OnInitialized/OnInitializedAsync
    // Called when component is first initialized
    protected override async Task OnInitializedAsync()
    {
        // Load data from database
        await LoadOrders();
    }

    // 2. OnParametersSet/OnParametersSetAsync
    // Called when parameters are set or changed
    
    // 3. StateHasChanged
    // Manually trigger UI refresh
    StateHasChanged();
    
    // 4. Dispose/DisposeAsync
    // Clean up resources (SignalR connections, etc.)
    public async ValueTask DisposeAsync()
    {
        await hubConnection.DisposeAsync();
    }
}
```

## Authentication Flow

### Registration Flow:
```
User fills registration form
    ↓
Submit → Register.razor
    ↓
UserManager.CreateAsync(user, password)
    ↓
Password hashed and stored
    ↓
UserManager.AddToRoleAsync(user, "User")
    ↓
SignInManager.SignInAsync(user)
    ↓
Redirect to Home page
```

### Login Flow:
```
User enters credentials
    ↓
Submit → Login.razor
    ↓
SignInManager.PasswordSignInAsync(email, password)
    ↓
Verify password hash
    ↓
Create authentication cookie
    ↓
AuthenticationStateProvider updates
    ↓
Redirect to Home page
```

### Authorization:
```razor
@attribute [Authorize(Roles = "Admin")]  // Page level

<AuthorizeView Roles="Admin">           // Component level
    <Authorized>
        <!-- Content for admins only -->
    </Authorized>
    <NotAuthorized>
        <!-- Content for non-admins -->
    </NotAuthorized>
</AuthorizeView>
```

## Database Operations

### EF Core Patterns Used:

**1. Async/Await (Recommended):**
```csharp
var orders = await DbContext.Orders
    .Where(o => o.UserId == userId)
    .ToListAsync();
```

**2. Include (Eager Loading):**
```csharp
var orders = await DbContext.Orders
    .Include(o => o.User)  // Load related user data
    .ToListAsync();
```

**3. Add and SaveChanges:**
```csharp
DbContext.Orders.Add(newOrder);
await DbContext.SaveChangesAsync();
```

**4. Update and SaveChanges:**
```csharp
order.Status = "Completed";
await DbContext.SaveChangesAsync();
```

## Configuration Files

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=Data/DeliveryApp.db"
  }
}
```

### DeliveryApp.csproj (NuGet Packages)
- Microsoft.AspNetCore.Identity.EntityFrameworkCore 9.0.0
- Microsoft.AspNetCore.SignalR.Client 9.0.0
- Microsoft.EntityFrameworkCore.Sqlite 9.0.0
- Microsoft.EntityFrameworkCore.Tools 9.0.0

## Security Considerations

### Current Implementation (Development):
✅ Password hashing (PBKDF2)
✅ Anti-forgery tokens
✅ Role-based authorization
✅ HTTPS redirection
⚠️ Simplified password requirements (for testing)

### Production Recommendations:
1. **Strengthen password requirements:**
   ```csharp
   options.Password.RequireDigit = true;
   options.Password.RequiredLength = 8;
   options.Password.RequireUppercase = true;
   options.Password.RequireNonAlphanumeric = true;
   ```

2. **Enable email confirmation:**
   ```csharp
   options.SignIn.RequireConfirmedAccount = true;
   ```

3. **Add rate limiting for login attempts**

4. **Use HTTPS only (remove HTTP endpoint)**

5. **Implement proper logging and monitoring**

6. **Add data validation and sanitization**

## Performance Optimizations

### Current Optimizations:
1. **SignalR Connection Pooling**: Reuses connections
2. **EF Core Compiled Queries**: (Can be added for frequently-used queries)
3. **CSS in single file**: Reduces HTTP requests
4. **Server-side rendering**: No large JavaScript bundles

### Potential Future Optimizations:
1. **Response caching** for static content
2. **Database indexing** on UserId and Status columns
3. **Pagination** for large order lists
4. **Connection multiplexing** for SignalR

## Testing Strategy

### Unit Testing (Recommended):
```csharp
// Test order creation logic
// Test status transitions
// Test authentication logic
```

### Integration Testing:
```csharp
// Test database operations
// Test SignalR connections
// Test authentication flow
```

### End-to-End Testing:
```
// Test complete user workflows
// Test admin workflows
// Test real-time updates
```

## Deployment

### Development:
```bash
dotnet run
```

### Production:
```bash
dotnet publish -c Release
```

**Hosting Options:**
- Azure App Service
- AWS Elastic Beanstalk
- DigitalOcean App Platform
- Self-hosted IIS/Kestrel
- Docker container

## Scaling Considerations

### Current Limitations (Single Server):
- All SignalR connections to one server
- In-memory session state
- Single SQLite database file

### Scaling Options:
1. **Redis backplane** for SignalR (multiple servers)
2. **SQL Server/PostgreSQL** instead of SQLite
3. **Load balancer** with sticky sessions
4. **Separate API layer** for microservices

## Common Patterns & Best Practices

### 1. Dependency Injection
```csharp
@inject ApplicationDbContext DbContext
@inject UserManager<ApplicationUser> UserManager
@inject NavigationManager Navigation
```

### 2. Async/Await
```csharp
// Always use async for I/O operations
await DbContext.SaveChangesAsync();
await hubConnection.StartAsync();
```

### 3. Using Statements
```csharp
// Proper resource disposal
using (var scope = app.Services.CreateScope())
{
    // Use services
}
```

### 4. Error Handling
```csharp
try
{
    await SaveData();
}
catch (Exception ex)
{
    logger.LogError(ex, "Error message");
    errorMessage = "User-friendly error message";
}
```

## Troubleshooting

### Common Issues:

**1. Database locked errors:**
- SQLite doesn't support high concurrency
- Consider SQL Server for production

**2. SignalR connection drops:**
- Check WebSocket support on hosting platform
- Verify firewall settings

**3. Authentication not persisting:**
- Check cookie settings
- Verify HTTPS configuration

**4. Real-time updates not working:**
- Ensure SignalR JavaScript library loaded
- Check browser console for errors
- Verify hub connection established

## Resources

- [.NET 9 Documentation](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-9)
- [Blazor Documentation](https://learn.microsoft.com/aspnet/core/blazor)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [ASP.NET Core Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity)
- [SignalR](https://learn.microsoft.com/aspnet/core/signalr)

---

**Last Updated:**  2025  
**.NET Version:** 9.0  
**Author:** Peter 
