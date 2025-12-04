# 🚚 Delivery App

A fullstack delivery management system built with .NET 9 Blazor Server,
featuring real-time order tracking, user authentication, and role-based access control.

## ✨ Features

### 👤 User Features
- **Authentication**: Secure sign up and login
- **Create Orders**: Submit delivery requests with:
  - Detailed description (e.g., "K50 for 2L Milk and K5 for a pen")
  - Location selection (Town, VML Market, Campus, Mukuba Mall)
  - Time needed (e.g., "2:30 PM", "14:30")
- **Order Dashboard**: View all personal orders with real-time status updates
- **Status Tracking**: Monitor orders through Pending → In Progress → Completed
- **Order Cancellation**: Orders can be cancelled if they are still pending

### 🧑‍💼 Admin Features
- **Admin Dashboard**: View all orders from all users
- **Order Management**: Accept and process orders
- **Status Updates**: Change order status with real-time notifications
- **Filter Orders**: View orders by status (All, Pending, In Progress, Completed)

### 🎨 UI/UX
- Clean and minimal design with "true blue" color theme
- Fully responsive (mobile and desktop)
- Real-time updates using SignalR
- Intuitive navigation and user experience

## 🛠️ Technology Stack

- **Framework**: .NET 9 Blazor Server
- **Database**: SQLite with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Real-time**: SignalR
- **Styling**: Custom CSS with responsive design

## 📋 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- A code editor (Visual Studio, VS Code, or Rider)

## 🚀 Getting Started

### 1. Clone or Navigate to the Project

```bash
"https://github.com/GlitchGuru19/WholeLottaDeliveries"
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Create and Apply Database Migrations

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

> **Note**: The database (`DeliveryApp.db`) will be automatically created in the `Data/` folder and seeded with an admin user on first run.

### 4. Run the Application

```bash
dotnet run
```

The application will start and be available at:
- HTTPS: `https://localhost:7000` (or similar)
- HTTP: `http://localhost:5000` (or similar)

Check the console output for the exact URLs.

## 👥 Default Credentials

### Admin Account
- **Email**: `admin@delivery.com`
- **Password**: `Admin123`

### User Accounts
Users can register their own accounts through the Register page.

## 📱 How to Use

### For Users:

1. **Register**: Create a new account at `/register`
2. **Login**: Sign in at `/login`
3. **Create Order**: 
   - Navigate to "New Order" or `/create-order`
   - Enter order description
   - Select delivery location
   - Choose estimated time needed
   - Submit order
4. **View Orders**: Check your orders at "My Orders" or `/dashboard`
5. **Track Status**: Watch real-time status updates

### For Admin:

1. **Login**: Use admin credentials at `/login`
2. **View Dashboard**: Access admin dashboard at `/admin`
3. **Filter Orders**: Use tabs to filter by status
4. **Accept Orders**: Click "Accept Order" to change status from Pending → In Progress
5. **Complete Orders**: Click "Mark as Completed" to finish orders

## 📂 Project Structure

```
DeliveryApp/
├── Components/
│   ├── Layout/
│   │   └── MainLayout.razor          # Main layout with navigation
│   ├── Pages/
│   │   ├── Home.razor                # Landing page
│   │   ├── Login.razor               # Login page
│   │   ├── Register.razor            # Registration page
│   │   ├── CreateOrder.razor         # Order creation form
│   │   ├── UserDashboard.razor       # User orders view
│   │   ├── AdminDashboard.razor      # Admin orders management
│   │   └── Logout.razor              # Logout handler
│   ├── App.razor                     # Root component
│   ├── Routes.razor                  # Routing configuration
│   └── _Imports.razor                # Global using statements
├── Data/
│   ├── ApplicationUser.cs            # User entity
│   ├── Order.cs                      # Order entity
│   └── ApplicationDbContext.cs       # Database context
├── Hubs/
│   └── OrderHub.cs                   # SignalR hub for real-time updates
├── wwwroot/
│   └── app.css                       # Application styles
├── Program.cs                        # Application entry point
├── appsettings.json                  # Configuration
└── DeliveryApp.csproj                # Project file
```

## 🔧 Configuration

Database connection string can be modified in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=Data/DeliveryApp.db"
  }
}
```

## 🎨 Customization

### Color Theme
The "true blue" theme can be customized in `wwwroot/app.css` by modifying the CSS variables:

```css
:root {
    --primary-blue: #0066CC;
    --dark-blue: #004C99;
    --light-blue: #E6F2FF;
    /* ... other colors */
}
```

### Locations
To add or modify delivery locations, 
update the select options in `Components/Pages/CreateOrder.razor`:

```razor
<InputSelect id="location" @bind-Value="orderModel.Location" class="form-control">
    <option value="">-- Select Location --</option>
    <option value="Town">Town</option>
    <option value="VML Market">VML Market</option>
    <option value="Campus">Campus</option>
    <option value="Mukuba Mall">Mukuba Mall</option>
    <!-- Add more locations here -->
</InputSelect>
```

## 🔒 Security Notes

- Passwords are hashed using ASP.NET Core Identity
- Authentication is required for all order operations
- Role-based authorization separates User and Admin access
- For production, use stronger password requirements and HTTPS

## 🚀 Real-time Features

The app uses SignalR for real-time updates:
- Order status changes are broadcast to all connected clients
- Both user and admin dashboards update automatically
- No page refresh needed to see latest changes

## 🐛 Troubleshooting

### Database Issues
If you encounter database errors:
```bash
dotnet ef database drop
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Port Conflicts
If the default ports are in use, modify `Properties/launchSettings.json` or use:
```bash
dotnet run --urls "https://localhost:7001;http://localhost:5001"
```

### SignalR Connection Issues
Ensure the SignalR CDN script is loading. Check browser console for errors.

## 📝 Notes

- Payments are handled in person (no payment integration)
- The SQLite database file (`DeliveryApp.db`) will be creatd in the Data folder 
- Admin user is created automatically on first run
- All timestamps are in local time

## 🤝 Contributing

Feel free to enhance this application by:
- Adding more delivery locations
- Implementing notification systems (email/SMS)
- Adding order history and analytics
- Enhancing the UI with animations
- Adding order cancellation feature
- Implementing delivery driver assignment

## 📄 License

This project is open source and available for educational purposes.

## 💬 Support

For issues or questions, please check the troubleshooting section or review the code comments.

---

**Built with ❤️ using .NET 9 Blazor Server**
