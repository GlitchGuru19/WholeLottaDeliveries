# 🚀 Quick Start Guide

## Application is Now Running! 🎉

Your Delivery App is live at: **http://localhost:5000**

## 👤 Login Credentials

### Admin Account
- **Email**: `admin@delivery.com`
- **Password**: `Admin123`

### User Account
Register a new user account through the Register page.

## 🎯 Quick Test Workflow

### 1. Test Admin Features:
1. Navigate to http://localhost:5000
2. Click "Login"
3. Use admin credentials above
4. View the Admin Dashboard
5. You'll see all orders from all users here

### 2. Test User Features:
1. Click "Logout" from admin account
2. Click "Register" to create a new user account
3. After registration, you'll be logged in automatically
4. Click "Create New Order"
5. Fill in:
   - Description: `K50 for 2L Milk and K5 for a pen`
   - Location: Select from dropdown (Town, VML Market, Campus, or Mukuba Mall)
   - Estimated Time: Choose when you need the items
6. Submit the order
7. View your order on "My Orders" dashboard

### 3. Test Admin Order Management:
1. Logout from user account
2. Login as admin again
3. Go to Admin Dashboard
4. See the new order from the user
5. Click "Accept Order" to change status to "In Progress"
6. Click "Mark as Completed" when done

### 4. Test Real-time Updates:
1. Open two browser windows
2. Login as user in one window
3. Login as admin in another
4. Admin accepts/completes an order
5. Watch the user's dashboard update in real-time without refresh!

## 📊 Features Overview

### User Features:
- ✅ Register and Login
- ✅ Create delivery orders with description, location, and time
- ✅ View personal orders dashboard
- ✅ Real-time status updates
- ✅ Track order status (Pending → In Progress → Completed)

### Admin Features:
- ✅ View all orders from all users
- ✅ Filter orders by status
- ✅ Accept pending orders
- ✅ Mark orders as completed
- ✅ Real-time updates across all clients

### UI Features:
- ✅ True blue color theme
- ✅ Responsive design (mobile & desktop)
- ✅ Clean, minimal interface
- ✅ Intuitive navigation

## 🗄️ Database

The SQLite database `DeliveryApp.db` has been created in the `Data/` folder with:
- User accounts and authentication data
- Order information
- Role assignments (Admin/User)

## 🔧 Available Commands

### Run the application:
```bash
dotnet run
```

### Stop the application:
Press `Ctrl+C` in the terminal

### View/Modify database:
The database is a SQLite file at: `D:\Everything C#\Blazor\DeliveryApp\Data\DeliveryApp.db`

You can use tools like [DB Browser for SQLite](https://sqlitebrowser.org/) to view/edit it directly.

### Reset database:
```bash
dotnet ef database drop
dotnet ef database update
```

## 📱 Access URLs

- **Home**: http://localhost:5000
- **Login**: http://localhost:5000/login
- **Register**: http://localhost:5000/register
- **User Dashboard**: http://localhost:5000/dashboard
- **Create Order**: http://localhost:5000/create-order
- **Admin Dashboard**: http://localhost:5000/admin

## 🎨 Customization

### Add More Locations:
Edit `Components/Pages/CreateOrder.razor` (line 30-35) to add locations:
```razor
<option value="New Location">New Location</option>
```

### Change Colors:
Edit `wwwroot/app.css` (line 2-12) to modify the color theme:
```css
:root {
    --primary-blue: #0066CC;  /* Change this */
    --dark-blue: #004C99;     /* And this */
}
```

## ❓ Troubleshooting

### Port Already in Use?
```bash
dotnet run --urls "http://localhost:5001"
```

### Database Errors?
```bash
dotnet ef database drop
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Can't Login as Admin?
The admin account is created automatically on first run. Check the console logs for any errors.

## 🎉 Enjoy Your Delivery App!

Everything is set up and ready to use. Have fun testing all the features!
