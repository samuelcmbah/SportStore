# SportStore: An ASP.NET Core E-Commerce Application

![.NET 8](https://img.shields.io/badge/.NET-8-blueviolet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-blue)
![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-brightgreen)
![Entity Framework Core](https://img.shields.io/badge/ORM-EF_Core-orange)

> SportStore is a full-featured e-commerce application built with ASP.NET Core MVC. As one of my foundational .NET projects, It demonstrates a realistic web application lifecycle: user authentication, session-backed shopping cart, data persistence with EF Core and PostgreSQL, transactional emails, and an administration area for managing products, categories, orders and roles..
<!--
### ✨ **[View the Live Demo Here](https://sportstore-67nv.onrender.com/)** ✨

---

<!-- 
  **Pro-Tip:** Add a high-quality screenshot or a GIF of your application in action here. It makes a huge difference!
  <p align="center">
    <img src="path/to/your/screenshot.png" alt="SportStore Application Screenshot" width="800"/>
  </p>
-->

## Key Features

-   **Product Catalog:** Browse products with clean pagination and filtering by category.
-   **Secure User Management:** Full user registration and login functionality powered by ASP.NET Core Identity.
-   **Email Confirmation:** New users must confirm their email address before they can log in.
-   **Order Confirmation Emails:** Customers now receive an order confirmation email after a successful checkout (includes order details and items).
-   **Hybrid Shopping Cart:** A seamless cart experience that persists items in the browser session for guest users and saves them to the database for logged-in users.
-   **Full Cart Functionality:** Easily add items to and remove items from the shopping cart.
-   **Order & Checkout:** A complete checkout process and an order history page for authenticated users.
-   **Admin Panel:**
   - CRUD for Category entity (create, read, update, delete)
   - CRUD for Product entity
   - Create Product form includes a Category dropdown
   - Admin-specific layout and navigation for a consistent admin UX

## Technology Stack

This project is built on a modern, robust technology stack:

-   **Backend:** **ASP.NET Core 8 MVC** for building the web application.
-   **Database:** **PostgreSQL** for reliable and scalable data storage.
-   **ORM:** **Entity Framework Core** to map C# objects to the database schema.
-   **Authentication:** **ASP.NET Core Identity** for handling all user management and security.
-   **Frontend:** **Razor Views** with **Bootstrap** for a responsive and clean UI.
-   **Logging:** **Serilog** for structured and file-based logging.
-   **Email Service:** **Resend API** for sending transactional emails (like registration confirmation and order confirmations).

## What’s New (Order Confirmation Emails)

This repository includes several recent improvements focused on admin usability and order reliability:

- Implemented CRUD for the Category entity and integrated category management into the Admin area.
- Product creation/edit forms show a Category dropdown so products can be assigned to categories.
- Admin-specific layout and startup adjustments to ensure the admin UI loads with a consistent navigation and styling.
- Implemented a product search bar (global) that supports searching by product name, category, or description.
- Fixed checkout and order persistence flow so order confirmation emails reliably include the persisted order and its items.

## Getting Started

Follow these instructions to get a local copy of the project up and running for development and testing purposes.

### Prerequisites

You will need the following software installed on your machine:

-   [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
-   [PostgreSQL Server](https://www.postgresql.org/download/)

### Installation & Setup

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/your-username/your-repo-name.git
    cd your-repo-name
    ```

2.  **Configure User Secrets:**
    This project uses `appsettings.json` for configuration. To keep your database connection strings and API keys secure, you should create an `appsettings.Development.json` file in the main project directory.

    Create a file named `appsettings.Development.json` and paste the following code into it, filling in your own values.
    ```json
    {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Warning"
        }
      },
      "ConnectionStrings": {
        "SportsStoreConnection": "Host=localhost;Database=your_db_name;Username=your_username;Password=your_password"
      },
      "ResendEmailSettings": {
        "ApiKey": "your_resend_api_key",
        "FromEmail": "your_sender_email@example.com"
      }
    }
    ```

    Note: The Resend API key and FromEmail are required for sending both user registration confirmation emails and order confirmation emails.

3.  **Apply Database Migrations:**
    Navigate to the project directory in your terminal and run the following command to create the database schema:
    ```bash
    dotnet ef database update
    ```

4.  **Run the Application:**
    You can now run the application using the .NET CLI:
    ```bash
    dotnet run
    ```
    The application will be available at `https://localhost:7000` or a similar address.

### Testing Order Emails Locally

- Ensure `ResendEmailSettings.ApiKey` and `ResendEmailSettings.FromEmail` are set in `appsettings.Development.json`.
- Use a testing email address or Resend’s sandbox/testing features to avoid spamming real users while developing.

## Roadmap & Future Improvements

I plan to continue developing this project with the following features:

-   [x] **Order Confirmation Emails:** Customers now receive email confirmations after checkout.
-   [ ] **Payment Gateway Integration:** Integrate a payment provider like Paystack to handle real transactions.
-   [ ] **PDF Receipts:** Automatically generate and email a PDF receipt to the user upon successful checkout.
-   [ ] **Product Reviews:** Allow users to write reviews and give ratings for products.
- [ ] Asynchronous background handling for emails (e.g., queues/workers)

## Contributing

Contributions and fixes are welcome. Please open issues or pull requests for bug fixes and feature requests.

---

*This project was created as a learning exercise and a portfolio piece. Feel free to explore the code!*
