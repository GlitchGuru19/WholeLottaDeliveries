# 🔐 Authentication Guide - Reusing Login & Register

## Overview

This guide explains how to reuse the existing authentication system in the Delivery App and customize redirects to different pages after login/registration.

---

## Table of Contents
1. [Understanding the Authentication System](#understanding-the-authentication-system)
2. [Reusing Login Component](#reusing-login-component)
3. [Reusing Register Component](#reusing-register-component)
4. [Customizing Redirects](#customizing-redirects)
5. [Protecting Pages](#protecting-pages)
6. [Common Scenarios](#common-scenarios)

---

## Understanding the Authentication System

### Components Involved

```
Program.cs
    ├── ASP.NET Core Identity (User Management)
    ├── Authentication State Provider (Session Management)
    └── Role Management (Admin/User)

Login.razor
    └── SignInManager (Handles login)

Register.razor
    └── UserManager (Creates users)

RedirectToLogin.razor
    └── Redirects unauthorized users
```

### Flow Diagram

```
User Visits Protected Page
        ↓
    Authorized?
        ↓
    NO → RedirectToLogin.razor → /login
        ↓
    Login.razor
        ↓
    SignInManager.PasswordSignInAsync()
        ↓
    Success → Navigation.NavigateTo("/your-page")
```

---

## Reusing Login Component

### Current Login Implementation

The login component is located at: `Components/Pages/Login.razor`

**Key Parts:**

```razor
@page "/login"
@inject SignInManager<ApplicationUser> SignInManager
@inject NavigationManager Navigation

@code {
    private async Task HandleLogin()
    {
        var result = await SignInManager.PasswordSignInAsync(
            loginModel.Email, 
            loginModel.Password, 
            isPersistent: false, 
            lockoutOnFailure: false
        );
        
        if (result.Succeeded)
        {
            Navigation.NavigateTo("/", forceLoad: true);
        }
    }
}
```

### Method 1: Redirect Based on User Role

Automatically redirect admins and users to different pages:

```razor
@code {
    private async Task HandleLogin()
    {
        var result = await SignInManager.PasswordSignInAsync(
            loginModel.Email, 
            loginModel.Password, 
            isPersistent: false, 
            lockoutOnFailure: false
        );
        
        if (result.Succeeded)
        {
            // Get the logged-in user
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            
            // Check role and redirect accordingly
            if (user.IsInRole("Admin"))
            {
                Navigation.NavigateTo("/admin", forceLoad: true);
            }
            else if (user.IsInRole("User"))
            {
                Navigation.NavigateTo("/dashboard", forceLoad: true);
            }
            else
            {
                Navigation.NavigateTo("/", forceLoad: true);
            }
        }
    }
}
```

**Don't forget to inject:**
```razor
@inject AuthenticationStateProvider AuthenticationStateProvider
```

### Method 2: Redirect to Specific Page

Redirect all users to a specific page after login:

```razor
@code {
    private async Task HandleLogin()
    {
        var result = await SignInManager.PasswordSignInAsync(
            loginModel.Email, 
            loginModel.Password, 
            isPersistent: false, 
            lockoutOnFailure: false
        );
        
        if (result.Succeeded)
        {
            // Redirect to your custom page
            Navigation.NavigateTo("/welcome", forceLoad: true);
        }
    }
}
```

### Method 3: Redirect with Return URL

Return to the page user was trying to access:

```razor
@page "/login"
@inject SignInManager<ApplicationUser> SignInManager
@inject NavigationManager Navigation

@code {
    [SupplyParameterFromQuery]
    public string? ReturnUrl { get; set; }
    
    private async Task HandleLogin()
    {
        var result = await SignInManager.PasswordSignInAsync(
            loginModel.Email, 
            loginModel.Password, 
            isPersistent: false, 
            lockoutOnFailure: false
        );
        
        if (result.Succeeded)
        {
            // Redirect to return URL or home page
            var redirectUrl = ReturnUrl ?? "/";
            Navigation.NavigateTo(redirectUrl, forceLoad: true);
        }
    }
}
```

**Usage:**
When redirecting to login, include return URL:
```csharp
Navigation.NavigateTo($"/login?returnUrl={Uri.EscapeDataString(currentUrl)}");
```

---

## Reusing Register Component

### Current Register Implementation

Located at: `Components/Pages/Register.razor`

```razor
@code {
    private async Task HandleRegister()
    {
        var user = new ApplicationUser 
        { 
            UserName = registerModel.Email, 
            Email = registerModel.Email 
        };
        
        var result = await UserManager.CreateAsync(user, registerModel.Password);
        
        if (result.Succeeded)
        {
            await UserManager.AddToRoleAsync(user, "User");
            await SignInManager.SignInAsync(user, isPersistent: false);
            Navigation.NavigateTo("/", forceLoad: true);
        }
    }
}
```

### Method 1: Custom Redirect After Registration

```razor
@code {
    private async Task HandleRegister()
    {
        var user = new ApplicationUser 
        { 
            UserName = registerModel.Email, 
            Email = registerModel.Email 
        };
        
        var result = await UserManager.CreateAsync(user, registerModel.Password);
        
        if (result.Succeeded)
        {
            // Add user to role
            await UserManager.AddToRoleAsync(user, "User");
            
            // Sign in the user
            await SignInManager.SignInAsync(user, isPersistent: false);
            
            // Redirect to onboarding or welcome page
            Navigation.NavigateTo("/onboarding", forceLoad: true);
        }
    }
}
```

### Method 2: Registration Without Auto-Login

Require email confirmation before login:

```razor
@code {
    private async Task HandleRegister()
    {
        var user = new ApplicationUser 
        { 
            UserName = registerModel.Email, 
            Email = registerModel.Email,
            EmailConfirmed = false  // Require confirmation
        };
        
        var result = await UserManager.CreateAsync(user, registerModel.Password);
        
        if (result.Succeeded)
        {
            await UserManager.AddToRoleAsync(user, "User");
            
            // DON'T auto-sign in
            // Show message and redirect to login
            successMessage = "Account created! Please check your email to confirm.";
            
            await Task.Delay(2000);
            Navigation.NavigateTo("/login", forceLoad: true);
        }
    }
}
```

### Method 3: Register with Additional Data

Capture extra information during registration:

```razor
@code {
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        // Additional fields
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }
    
    private async Task HandleRegister()
    {
        var user = new ApplicationUser 
        { 
            UserName = registerModel.Email, 
            Email = registerModel.Email,
            PhoneNumber = registerModel.PhoneNumber  // Store additional data
        };
        
        var result = await UserManager.CreateAsync(user, registerModel.Password);
        
        if (result.Succeeded)
        {
            await UserManager.AddToRoleAsync(user, "User");
            await SignInManager.SignInAsync(user, isPersistent: false);
            Navigation.NavigateTo("/profile-setup", forceLoad: true);
        }
    }
}
```

---

## Customizing Redirects

### Scenario 1: Multi-Step Onboarding

Create an onboarding flow after registration:

**Step 1: Register** → **Step 2: Profile Setup** → **Step 3: Preferences** → **Dashboard**

```razor
@page "/onboarding/step1"
@attribute [Authorize]

<h2>Welcome! Let's set up your profile</h2>

<EditForm Model="profileModel" OnValidSubmit="HandleStep1">
    <InputText @bind-Value="profileModel.FullName" />
    <button type="submit">Next</button>
</EditForm>

@code {
    private void HandleStep1()
    {
        // Save profile data
        // Redirect to next step
        Navigation.NavigateTo("/onboarding/step2");
    }
}
```

### Scenario 2: Role-Based Landing Pages

Redirect users to different pages based on their role:

```razor
@page "/redirect-handler"
@inject AuthenticationStateProvider AuthenticationStateProvider
@inject NavigationManager Navigation

@code {
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        
        if (user.IsInRole("Admin"))
        {
            Navigation.NavigateTo("/admin/dashboard");
        }
        else if (user.IsInRole("User"))
        {
            Navigation.NavigateTo("/user/dashboard");
        }
        else if (user.IsInRole("Vendor"))
        {
            Navigation.NavigateTo("/vendor/orders");
        }
        else
        {
            Navigation.NavigateTo("/");
        }
    }
}
```

**Update Login.razor:**
```razor
if (result.Succeeded)
{
    Navigation.NavigateTo("/redirect-handler", forceLoad: true);
}
```

### Scenario 3: Remember Last Visited Page

Store and return to the last visited page:

```razor
@inject IJSRuntime JSRuntime

@code {
    private async Task HandleLogin()
    {
        var result = await SignInManager.PasswordSignInAsync(
            loginModel.Email, 
            loginModel.Password, 
            isPersistent: false, 
            lockoutOnFailure: false
        );
        
        if (result.Succeeded)
        {
            // Get last visited page from localStorage
            var lastPage = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "lastPage");
            
            if (!string.IsNullOrEmpty(lastPage))
            {
                Navigation.NavigateTo(lastPage, forceLoad: true);
            }
            else
            {
                Navigation.NavigateTo("/", forceLoad: true);
            }
        }
    }
}
```

---

## Protecting Pages

### Method 1: Require Authentication

Require any authenticated user:

```razor
@page "/protected-page"
@attribute [Authorize]

<h2>This page requires login</h2>
```

### Method 2: Require Specific Role

Only allow specific roles:

```razor
@page "/admin-only"
@attribute [Authorize(Roles = "Admin")]

<h2>Admin Only Page</h2>
```

### Method 3: Require Multiple Roles

Allow multiple roles (OR condition):

```razor
@page "/dashboard"
@attribute [Authorize(Roles = "Admin,Manager,User")]

<h2>Dashboard</h2>
```

### Method 4: Conditional Content

Show different content based on authentication:

```razor
@page "/home"

<AuthorizeView>
    <Authorized>
        <h2>Welcome back, @context.User.Identity.Name!</h2>
    </Authorized>
    <NotAuthorized>
        <h2>Welcome! Please log in.</h2>
    </NotAuthorized>
</AuthorizeView>

<AuthorizeView Roles="Admin">
    <Authorized>
        <a href="/admin">Admin Panel</a>
    </Authorized>
</AuthorizeView>
```

### Method 5: Programmatic Authorization Check

Check authorization in code:

```razor
@inject AuthenticationStateProvider AuthenticationStateProvider

@code {
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        
        if (!user.Identity.IsAuthenticated)
        {
            Navigation.NavigateTo("/login");
            return;
        }
        
        if (!user.IsInRole("Admin"))
        {
            Navigation.NavigateTo("/access-denied");
            return;
        }
        
        // User is authenticated and is admin
        await LoadAdminData();
    }
}
```

---

## Common Scenarios

### Scenario 1: New User Onboarding Flow

```
Register → Email Confirmation → Profile Setup → Tutorial → Dashboard
```

**Implementation:**

1. **Register.razor** - Create account
```razor
if (result.Succeeded)
{
    Navigation.NavigateTo("/onboarding/welcome", forceLoad: true);
}
```

2. **OnboardingWelcome.razor** - Welcome message
```razor
@page "/onboarding/welcome"
@attribute [Authorize]

<h2>Welcome to Delivery App!</h2>
<button @onclick="() => Navigation.NavigateTo('/onboarding/profile')">
    Continue
</button>
```

3. **OnboardingProfile.razor** - Collect additional info
```razor
@page "/onboarding/profile"
@attribute [Authorize]

<EditForm Model="profileModel" OnValidSubmit="SaveProfile">
    <!-- Profile fields -->
    <button type="submit">Continue</button>
</EditForm>

@code {
    private void SaveProfile()
    {
        // Save profile data
        Navigation.NavigateTo("/onboarding/tutorial");
    }
}
```

4. **OnboardingTutorial.razor** - Show tutorial
```razor
@page "/onboarding/tutorial"
@attribute [Authorize]

<h2>Quick Tutorial</h2>
<!-- Tutorial content -->
<button @onclick="() => Navigation.NavigateTo('/dashboard')">
    Get Started
</button>
```

### Scenario 2: Different Dashboards for Different Roles

**Login.razor:**
```razor
private async Task HandleLogin()
{
    var result = await SignInManager.PasswordSignInAsync(
        loginModel.Email, 
        loginModel.Password, 
        isPersistent: false, 
        lockoutOnFailure: false
    );
    
    if (result.Succeeded)
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        
        // Role-based redirect
        if (user.IsInRole("Admin"))
        {
            Navigation.NavigateTo("/admin/dashboard", forceLoad: true);
        }
        else if (user.IsInRole("Vendor"))
        {
            Navigation.NavigateTo("/vendor/dashboard", forceLoad: true);
        }
        else if (user.IsInRole("Customer"))
        {
            Navigation.NavigateTo("/customer/dashboard", forceLoad: true);
        }
        else
        {
            Navigation.NavigateTo("/", forceLoad: true);
        }
    }
}
```

### Scenario 3: Email Confirmation Required

**Program.cs:**
```csharp
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;  // Enable confirmation
    options.SignIn.RequireConfirmedEmail = true;
})
```

**Register.razor:**
```razor
private async Task HandleRegister()
{
    var user = new ApplicationUser 
    { 
        UserName = registerModel.Email, 
        Email = registerModel.Email,
        EmailConfirmed = false  // Needs confirmation
    };
    
    var result = await UserManager.CreateAsync(user, registerModel.Password);
    
    if (result.Succeeded)
    {
        await UserManager.AddToRoleAsync(user, "User");
        
        // Generate email confirmation token
        var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        
        // Send confirmation email (implement your email service)
        // await EmailService.SendConfirmationEmail(user.Email, token);
        
        successMessage = "Please check your email to confirm your account.";
        
        // Redirect to confirmation page
        Navigation.NavigateTo("/confirm-email-sent", forceLoad: true);
    }
}
```

### Scenario 4: Social Login Integration

Add Google/Facebook login alongside email/password:

**Program.cs:**
```csharp
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    })
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    });
```

**Login.razor:**
```razor
<h2>Login</h2>

<!-- Email/Password Login -->
<EditForm Model="loginModel" OnValidSubmit="HandleLogin">
    <!-- Form fields -->
</EditForm>

<!-- OR -->
<div class="social-login">
    <h3>Or login with:</h3>
    <form action="/Account/ExternalLogin" method="post">
        <button type="submit" name="provider" value="Google">
            Login with Google
        </button>
        <button type="submit" name="provider" value="Facebook">
            Login with Facebook
        </button>
    </form>
</div>
```

---

## Quick Reference

### Essential Injections

```razor
@inject UserManager<ApplicationUser> UserManager
@inject SignInManager<ApplicationUser> SignInManager
@inject AuthenticationStateProvider AuthenticationStateProvider
@inject NavigationManager Navigation
```

### Common Navigation Patterns

```csharp
// Simple redirect
Navigation.NavigateTo("/page");

// Force page reload (updates auth state)
Navigation.NavigateTo("/page", forceLoad: true);

// With query parameters
Navigation.NavigateTo($"/page?id={id}");

// With return URL
Navigation.NavigateTo($"/login?returnUrl={Uri.EscapeDataString(currentUrl)}");
```

### Check Authentication Status

```csharp
var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
var user = authState.User;

if (user.Identity.IsAuthenticated)
{
    // User is logged in
}

if (user.IsInRole("Admin"))
{
    // User is admin
}

var userName = user.Identity.Name;  // Get username
```

---

## Best Practices

1. **Always use `forceLoad: true`** when navigating after login/logout
2. **Validate user roles** before showing sensitive content
3. **Use `[Authorize]` attribute** to protect pages
4. **Store sensitive data** server-side, not in localStorage
5. **Implement return URLs** for better UX
6. **Show loading indicators** during authentication
7. **Handle errors gracefully** with user-friendly messages
8. **Use HTTPS** in production
9. **Implement rate limiting** for login attempts
10. **Log security events** for auditing

---

## Troubleshooting

### Authentication Not Working After Redirect

**Problem:** User appears not authenticated after redirect.

**Solution:** Use `forceLoad: true`:
```csharp
Navigation.NavigateTo("/dashboard", forceLoad: true);
```

### Can't Access User Information

**Problem:** `context.User.Identity.Name` is null.

**Solution:** Ensure you're inside an `<AuthorizeView>` or inject `AuthenticationStateProvider`:
```razor
@inject AuthenticationStateProvider AuthenticationStateProvider

@code {
    private string? userName;
    
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        userName = authState.User.Identity?.Name;
    }
}
```

### Redirect Loop

**Problem:** Page keeps redirecting to login.

**Solution:** Check that your return URL handling doesn't create a loop:
```csharp
// Bad - can create loop
Navigation.NavigateTo($"/login?returnUrl=/login");

// Good - check return URL
var returnUrl = ReturnUrl ?? "/";
if (returnUrl == "/login" || returnUrl == "/register")
{
    returnUrl = "/";
}
Navigation.NavigateTo(returnUrl, forceLoad: true);
```

---

## Examples Repository Structure

```
Components/
├── Pages/
│   ├── Auth/
│   │   ├── Login.razor              (Main login)
│   │   ├── Register.razor           (Main registration)
│   │   ├── ForgotPassword.razor     (Password reset)
│   │   └── ConfirmEmail.razor       (Email confirmation)
│   ├── Onboarding/
│   │   ├── Welcome.razor            (Step 1)
│   │   ├── Profile.razor            (Step 2)
│   │   └── Preferences.razor        (Step 3)
│   └── Dashboard/
│       ├── UserDashboard.razor      (For users)
│       └── AdminDashboard.razor     (For admins)
```

---

## Summary

✅ **Login/Register** components are fully reusable  
✅ **Customize redirects** by modifying `Navigation.NavigateTo()`  
✅ **Protect pages** with `[Authorize]` attribute  
✅ **Role-based redirects** using `user.IsInRole()`  
✅ **Multi-step flows** by chaining page navigations  
✅ **Return URLs** for better user experience  

---

**Last Updated:** January 2025  
**.NET Version:** 9.0  
**For questions, refer to:** `ARCHITECTURE.md` and `DOTNET9_GUIDE.md`
