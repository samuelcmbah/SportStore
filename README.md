# SportStore: An ASP.NET Core E-Commerce Application

![.NET 8](https://img.shields.io/badge/.NET-8-blueviolet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-blue)
![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-brightgreen)
![Entity Framework Core](https://img.shields.io/badge/ORM-EF_Core-orange)

> SportStore is a full-featured e-commerce application built with ASP.NET Core MVC. As one of my foundational .NET projects, it was built to demonstrate a practical understanding of C# and the ASP.NET Core ecosystem.

This project showcases a complete web application lifecycle, from user authentication and session management to data persistence and administration, serving as a comprehensive portfolio piece.

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
-   **Hybrid Shopping Cart:** A seamless cart experience that persists items in the browser session for guest users and saves them to the database for logged-in users.
-   **Full Cart Functionality:** Easily add items to and remove items from the shopping cart.
-   **Order & Checkout:** A complete checkout process and an order history page for authenticated users.
-   **Admin Panel:** A secure area for administrators to manage products in the store.

## Technology Stack

This project is built on a modern, robust technology stack:

-   **Backend:** **ASP.NET Core 8 MVC** for building the web application.
-   **Database:** **PostgreSQL** for reliable and scalable data storage.
-   **ORM:** **Entity Framework Core** to map C# objects to the database schema.
-   **Authentication:** **ASP.NET Core Identity** for handling all user management and security.
-   **Frontend:** **Razor Views** with **Bootstrap** for a responsive and clean UI.
-   **Logging:** **Serilog** for structured and file-based logging.
-   **Email Service:** **Resend API** for sending transactional emails (like registration confirmation).

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
    The application will be available at `https://localhost:5001` or a similar address.

## Roadmap & Future Improvements

I plan to continue developing this project with the following features:

-   [ ] **Payment Gateway Integration:** Integrate a payment provider like Paystack to handle real transactions.
-   [ ] **PDF Receipts:** Automatically generate and email a PDF receipt to the user upon successful checkout.
-   [ ] **Product Reviews:** Allow users to write reviews and give ratings for products.

---

*This project was created as a learning exercise and a portfolio piece. Feel free to explore the code!*
