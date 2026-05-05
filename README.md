🏥 Riaya Backend

Riaya is a healthcare system backend built with ASP.NET Core Web API using Clean Architecture.
It provides secure RESTful APIs for authentication, doctor and patient management, appointment booking, scheduling, and online payments through Paymob.

📌 Project Overview

Riaya is designed as a scalable backend foundation for healthcare platforms where patients can register, browse doctors, book appointments, and complete payments online.

The system follows a layered architecture to ensure maintainability, scalability, and testability.

🎯 Purpose

This project demonstrates building a production-ready backend system using Clean Architecture, focusing on real-world healthcare workflows such as booking, scheduling, and payment processing.

🧱 Architecture

The solution follows Clean Architecture principles:

Riaya
├── Riaya.API              # Presentation layer - Controllers, Middleware, Swagger
├── Riaya.Application      # Business logic, DTOs, Interfaces
├── Riaya.Domain           # Core entities and domain rules
├── Riaya.Infrastructure   # External services (JWT, Paymob, Seeders)
└── Riaya.Persistence      # Database access (EF Core, Migrations)
✨ Features
🔐 JWT Authentication & Authorization
👥 Role-based system (Admin, Doctor, Patient)
🧾 User registration, login, and token management
📅 Doctor schedules and time slot management
🩺 Appointment booking system
💳 Paymob payment integration
📦 Entity Framework Core with SQL Server
📚 Swagger API documentation
🧯 Global exception handling
🗃️ Database migrations & seeding
🧠 Key Concepts Applied
Clean Architecture (Separation of Concerns)
Dependency Injection
Repository Pattern
JWT Authentication Flow
External Payment Integration (Paymob)
🛠 Tech Stack
.NET 8
ASP.NET Core Web API
Entity Framework Core
SQL Server
JWT Bearer Authentication
FluentValidation
Swagger / Swashbuckle
Paymob API
🚀 Getting Started
1. Clone the Repository
git clone https://github.com/mohamedfaresss/Riaya-Backend-
cd Riaya-Backend-
2. Restore Dependencies
dotnet restore
3. Configure Environment Variables

⚠️ Do NOT commit real secrets to GitHub.

Example configuration:

{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY",
    "Issuer": "Riaya",
    "Audience": "Riaya"
  },
  "Paymob": {
    "ApiKey": "YOUR_API_KEY",
    "IntegrationId": "YOUR_INTEGRATION_ID",
    "IframeId": "YOUR_IFRAME_ID",
    "HmacSecret": "YOUR_HMAC_SECRET"
  }
}
4. Run the Application
dotnet run --project Riaya.API
📚 API Documentation

Swagger UI is available after running the project:

https://localhost:{PORT}/swagger
🧪 API Testing

You can test endpoints using:

Swagger UI
Postman
Any REST client
🔐 Security Notes
Never expose secrets (JWT, DB, Paymob keys)
Use environment variables in production
Enable HTTPS
Restrict CORS before deployment
🌐 Future Improvements
Add Unit & Integration Testing
Docker support
CI/CD pipeline
Logging with Serilog
API versioning
👨‍💻 Author

Mohamed Gamal Fares
Backend Developer (.NET)
