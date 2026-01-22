# SportStore: Professional ASP.NET Core E-Commerce Platform

![.NET 8](https://img.shields.io/badge/.NET-8-blueviolet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-blue)
![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-brightgreen)
![Entity Framework Core](https://img.shields.io/badge/ORM-EF_Core-orange)
![Bootstrap](https://img.shields.io/badge/UI-Bootstrap_5-purple)
![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)

> A production-ready e-commerce platform showcasing enterprise-level .NET development practices, clean architecture principles, and modern web application patterns. Built with ASP.NET Core MVC, this project demonstrates full-stack proficiency from database design to payment integration.

---

## 🎯 Project Highlights

This application represents a comprehensive implementation of modern web development practices:

- **Clean Architecture**: Separation of concerns with dedicated service layers, domain services, and repository patterns
- **Security-First Design**: ASP.NET Core Identity with email verification, role-based authorization, and CSRF protection
- **Payment Integration**: Full integration with external payment gateway (PayBridge/Paystack) including webhook handling
- **Real-Time Features**: Order status polling with JavaScript for live payment verification
- **Inventory Management**: Stock tracking with validation to prevent overselling
- **Responsive Design**: Mobile-first UI using Bootstrap 5 with custom CSS architecture
- **Production Logging**: Structured logging with Serilog for monitoring and debugging

---

## 🏗️ Technical Architecture

### Backend Architecture

```
┌─────────────────────────────────────────────────────┐
│                   Presentation Layer                │
│  (Controllers, Views, ViewModels, View Components)  │
└──────────────────┬──────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────┐
│                   Service Layer                     │
│  • Domain Services (Cart, Order, Inventory)         │
│  • Application Services (Product, Category, Auth)   │
│  • External Services (Email, Payment)               │
└──────────────────┬──────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────┐
│                 Data Access Layer                   │
│  • EF Core DbContext                                │
│  • Repository Pattern                               │
│  • Two separate contexts (Store & Identity)         │
└──────────────────┬──────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────┐
│                  PostgreSQL Database                │
└─────────────────────────────────────────────────────┘
```

### Key Design Patterns Implemented

- **Repository Pattern**: Abstracted data access with `IStoreRepository`, `IOrderRepository`
- **Domain-Driven Design**: Separated domain logic into `CartDomainService`, `OrderDomainService`, `InventoryService`
- **Dependency Injection**: Full utilization of ASP.NET Core's built-in DI container
- **Service Layer Pattern**: Clear separation between business logic and data access
- **ViewModel Pattern**: DTOs for transferring data between layers
- **Middleware Pipeline**: Custom middleware for role-based routing and error handling

---

## 💼 Core Features & Business Logic

### 1. Advanced Shopping Cart System
**Demonstrates:** State management, session handling, database synchronization

- **Dual-Mode Cart**: Session-based for anonymous users, database-persisted for authenticated users
- **Automatic Migration**: Seamless cart merging when users log in
- **Real-Time Updates**: AJAX-powered cart updates without page refresh
- **Stock Validation**: Real-time inventory checks before checkout

**Technical Implementation:**
```csharp
// Smart cart resolution based on authentication state
private async Task<Cart> GetCartAsync()
{
    if (User.Identity!.IsAuthenticated)
    {
        var userId = currentUserService.UserId!;
        return await cartService.GetOrCreateCartByUserIdAsync(userId);
    }
    return sessionCart.GetCart();
}
```

### 2. Payment Gateway Integration
**Demonstrates:** External API integration, webhook handling, async operations

- **PayBridge/Paystack Integration**: Full payment initialization and verification flow
- **Webhook Processing**: Secure payment notifications with idempotency checks
- **Order Status Polling**: Client-side JavaScript polling for real-time payment updates
- **Transaction Safety**: Inventory reduction only after successful payment verification

**Key Files:**
- `PaymentService.cs` - Payment initialization
- `PaymentNotificationAPIController.cs` - Webhook endpoint
- `order-status-poller.js` - Client-side polling

### 3. Inventory Management
**Demonstrates:** Transaction handling, race condition prevention, validation logic

- **Stock Validation**: Multi-level checks (cart → checkout → payment)
- **Atomic Operations**: Inventory reduction within transactions
- **Overselling Prevention**: Reserved quantity tracking
- **Error Recovery**: Proper handling of insufficient stock scenarios

```csharp
public async Task ReduceInventoryForOrderAsync(Order order)
{
    foreach (var orderItem in order.OrderItems)
    {
        var product = await context.Products.FindAsync(orderItem.ProductId);
        
        if (product.StockQuantity < orderItem.Quantity)
        {
            throw new InvalidOperationException(
                $"Insufficient stock for {product.Name}"
            );
        }
        
        product.StockQuantity -= orderItem.Quantity;
        logger.LogInformation("Reduced stock for {Product}", product.Name);
    }
    
    await context.SaveChangesAsync();
}
```

### 4. Comprehensive Admin Panel
**Demonstrates:** Authorization, CRUD operations, file uploads, search functionality

- **Role-Based Access**: Administrator-only area with policy-based authorization
- **Product Management**: Full CRUD with image uploads, category assignment, stock tracking
- **Category Management**: Independent category administration
- **Order Processing**: View orders, mark as shipped, track payment status
- **Role Management**: User-role assignment with custom view components
- **Search & Filter**: Server-side product search with multiple criteria

### 5. Email Notification System
**Demonstrates:** Third-party API integration, HTML email templating, async communication

- **Resend API Integration**: Production-ready email service
- **Email Verification**: Required email confirmation for new accounts
- **Order Confirmations**: Automatic emails with itemized order details
- **Template System**: HTML email templates with dynamic data

```csharp
public async Task SendOrderPlacedEmailAsync(Order order)
{
    var emailDto = MapToEmailDto(order);
    var html = BuildOrderPlacedHtml(emailDto);
    
    await emailService.SendEmailAsync(
        order.Email,
        $"Order #{order.OrderID} Confirmation",
        html
    );
}
```

---

## 🛠️ Technology Stack & Tools

### Backend Technologies
- **ASP.NET Core 8 MVC** - Latest LTS version with Minimal APIs for webhooks
- **Entity Framework Core 8** - Code-first migrations, Include/ThenInclude for eager loading
- **PostgreSQL** - Production-grade relational database
- **ASP.NET Core Identity** - Authentication & authorization framework
- **Serilog** - Structured logging with file sinks

### Frontend Technologies
- **Razor View Engine** - Server-side rendering with ViewComponents
- **Bootstrap 5** - Responsive UI framework
- **Custom CSS Architecture** - Component-based styling (buttons, badges, cards, sidebar)
- **Vanilla JavaScript** - AJAX operations, polling, dynamic updates
- **Bootstrap Icons** - Consistent iconography

### External Services
- **Resend API** - Transactional email delivery
- **PayBridge/Paystack** - Payment processing gateway

### Development Tools
- **User Secrets** - Secure configuration management
- **Serilog File Logging** - Production debugging
- **EF Core Migrations** - Database version control
- **Dependency Injection** - Built-in IoC container

---

## 📊 Database Schema

The application uses two separate `DbContext` instances for security and separation of concerns:

### StoreDbContext (Business Data)
```
Categories ──┐
             ├──< Products >──┬──< CartItems >──── Carts
                              └──< OrderItems >─── Orders
```

### AppIdentityDbContext (Authentication)
```
AspNetUsers ──┬──< AspNetUserRoles >──── AspNetRoles
              ├──< AspNetUserClaims >
              └──< AspNetUserLogins >
```

**Key Relationships:**
- Products have delete restrictions on both CartItems and OrderItems (prevents accidental data loss)
- Category deletion is restricted if products exist
- Cart has unique index on UserId (one cart per user)

---

## 🔐 Security Implementation

### Authentication & Authorization
- **Email Verification Required**: Users cannot log in until email is confirmed
- **Role-Based Authorization**: Admin area protected with `[Authorize(Roles = "Administrator")]`
- **Password Requirements**: Configured through ASP.NET Core Identity
- **Anti-Forgery Tokens**: CSRF protection on all POST forms
- **Secure Cookie Settings**: HttpOnly and SameSite policies

### Payment Security
- **Webhook Validation**: Verification of payment notifications
- **Idempotency**: Prevents duplicate order processing
- **Status Checks**: Multiple validation points before inventory reduction

### Data Protection
- **User Secrets**: Sensitive data excluded from source control
- **Connection String Security**: Database credentials in environment-specific config
- **API Key Management**: External service keys in secure configuration

---

## 🚀 Advanced Features

### 1. Smart Cart Management
- Session persistence for guest users
- Database persistence for authenticated users
- Automatic cart migration on login
- Merge logic prevents duplicate items

### 2. Order Processing Pipeline
```
Cart → Validation → Payment Init → Webhook → Inventory Reduction → Email
```

### 3. Real-Time Order Updates
JavaScript polling updates order status without page refresh:
```javascript
async function checkStatus() {
    const response = await fetch(`/api/orders/${orderId}/status`);
    const data = await response.json();
    
    if (data.status === 'Success' || data.status === 'Failed') {
        location.reload();
    }
}
```

### 4. Advanced Search & Filtering
- Full-text search across product name, description, and category
- Category filtering
- Price range filtering
- Combined filter support (search + category + price)

### 5. Responsive Product Grid
- Adaptive layout (2 cols mobile, 5 cols desktop)
- Lazy loading images
- Quick-add to cart without page navigation
- Stock status indicators

---

## 📁 Project Structure

```
SportStore/
├── Areas/
│   └── Admin/                 # Admin-only controllers and views
│       ├── Controllers/
│       │   ├── ProductsController.cs
│       │   ├── CategoryController.cs
│       │   ├── OrdersController.cs
│       │   └── RolesController.cs
│       └── Views/
├── Components/                # View Components (reusable UI)
│   ├── CartWidgetViewComponent.cs
│   ├── FilterPanelViewComponent.cs
│   └── NavigationMenuViewComponent.cs
├── Controllers/
│   ├── HomeController.cs      # Product browsing
│   ├── CartController.cs      # Cart operations
│   ├── OrderController.cs     # Checkout & order history
│   ├── AccountController.cs   # Auth operations
│   └── PaymentNotificationAPIController.cs  # Webhook endpoint
├── Data/
│   ├── StoreDbContext.cs      # Main database context
│   ├── AppIdentityDbContext.cs # Identity context
│   └── SeedData.cs            # Database seeding
├── Services/
│   ├── IServices/             # Service interfaces
│   ├── ProductService.cs      # Product business logic
│   ├── CartService.cs         # Cart database operations
│   ├── CartDomainService.cs   # Cart business rules
│   ├── OrderDomainService.cs  # Order creation logic
│   ├── InventoryService.cs    # Stock management
│   ├── PaymentService.cs      # Payment API integration
│   ├── EmailService.cs        # Email sending
│   └── CurrentUserService.cs  # User context access
├── ViewModels/                # DTOs for views
│   ├── ProductVM/
│   ├── CartVM/
│   ├── Auth/
│   └── EmailVM/
├── Models/                    # Domain entities
│   ├── Product.cs
│   ├── Category.cs
│   ├── Cart.cs
│   ├── Order.cs
│   └── ApplicationUser.cs
├── Middleware/
│   └── RoleBasedRootRedirectMiddleware.cs
└── wwwroot/
    ├── css/
    │   ├── site.css           # Global styles
    │   ├── components/        # Component-specific styles
    │   └── pages/             # Page-specific styles
    └── js/
        └── order-status-poller.js
```

---

## 🎨 UI/UX Implementation

### Design System
- **Custom CSS Variables**: Consistent color scheme and spacing
- **Component Architecture**: Modular CSS files (buttons.css, badges.css, cards.css)
- **Mobile-First Approach**: Responsive breakpoints for all components
- **Accessibility**: Semantic HTML, ARIA labels, keyboard navigation

### Key UI Components
- **Product Cards**: Hover effects, stock indicators, quick-add buttons
- **Shopping Cart**: Inline quantity updates, remove items, responsive layout
- **Checkout Flow**: Multi-step form with validation, order summary sidebar
- **Admin Tables**: Sortable, searchable product/order lists
- **Status Badges**: Color-coded order statuses (Pending, Success, Failed)

---

## 🧪 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 12+](https://www.postgresql.org/download/)
- [Resend Account](https://resend.com/) (for email functionality)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/sportstore.git
   cd sportstore
   ```

2. **Configure Application Settings**
   
   Create `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "SportsStoreConnection": "Host=localhost;Database=sportstore;Username=your_user;Password=your_password"
     },
     "ResendEmailSettings": {
       "ApiKey": "your_resend_api_key",
       "FromEmail": "noreply@yourdomain.com"
     },
     "ExternalServices": {
       "PayBridgeUrl": "https://localhost:7000"
     }
   }
   ```

3. **Apply Database Migrations**
   ```bash
   dotnet ef database update --context StoreDbContext
   dotnet ef database update --context AppIdentityDbContext
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```
   
   The application will be available at `https://localhost:7001`

5. **Default Admin Credentials**
   - Email: `admin@example.com`
   - Password: `Secret123$`

---

## 📚 Key Learnings & Achievements

### Technical Skills Demonstrated

1. **Full-Stack Development**
   - End-to-end feature implementation from database to UI
   - RESTful API design for webhooks
   - Client-server communication patterns

2. **Database Design**
   - Normalized schema design
   - Complex relationships (one-to-many, many-to-many)
   - Migration management and data seeding

3. **Security Best Practices**
   - Authentication and authorization implementation
   - Secure configuration management
   - CSRF protection and data validation

4. **External Integration**
   - Payment gateway integration
   - Email service integration
   - Webhook handling and verification

5. **Code Organization**
   - Clean architecture principles
   - Dependency injection
   - Service layer abstraction
   - Repository pattern

### Problem-Solving Examples

**Challenge**: Cart persistence across user states (guest → logged in)  
**Solution**: Implemented hybrid cart system with automatic migration

**Challenge**: Race conditions in inventory management  
**Solution**: Implemented atomic operations with transaction-level locking

**Challenge**: Payment verification timing  
**Solution**: Created client-side polling mechanism with timeout handling

---

## 🔮 Future Enhancements

### Planned Features
- [x] Payment integration (PayBridge/Paystack)
- [ ] Background job processing 
- [ ] Product reviews and ratings system
- [ ] PDF invoice generation
- [ ] Wishlist functionality
- [ ] Social media authentication

### Technical Improvements
- [ ] Implement CQRS pattern
- [ ] Add Redis caching layer
- [ ] API versioning
- [ ] Comprehensive unit tests
- [ ] Integration tests
- [ ] Docker containerization
- [ ] CI/CD pipeline

---

## 📝 Code Quality

This project demonstrates:
- **Clean Code Principles**: Meaningful names, single responsibility, DRY
- **SOLID Principles**: Especially dependency inversion and interface segregation
- **Error Handling**: Comprehensive try-catch blocks with proper logging
- **Logging Strategy**: Structured logging at appropriate levels (Info, Warning, Error)
- **Configuration Management**: Environment-specific settings
- **Code Documentation**: XML comments on public interfaces

---

## 🤝 Contributing

While this is a portfolio project, I welcome feedback and suggestions. Feel free to:
- Open issues for bugs or feature requests
- Submit pull requests for improvements
- Reach out for collaboration opportunities

---

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

---

## 👨‍💻 About the Developer

This project showcases my proficiency in:
- **.NET ecosystem and C# programming**
- **ASP.NET Core MVC architecture**
- **Entity Framework Core and database design**
- **RESTful API development**
- **Payment gateway integration**
- **Authentication and authorization**
- **Frontend development with modern CSS**
- **Problem-solving and system design**
* GitHub: [samuelcmbah](https://github.com/samuelcmbah)
* LinkedIn: [Samuel Mbah](https://linkedin.com/in/samuelcmbah)
---

**Note**: This is a demonstration project built for learning purposes and portfolio presentation. It represents production-ready code patterns and practices suitable for real-world applications.


