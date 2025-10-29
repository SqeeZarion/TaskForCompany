# 🐶 DogsHouseService

### English 🇬🇧
**DogsHouseService** is a RESTful API built with **.NET 9** that follows the **Onion Architecture** pattern.  
The project was developed as part of a **technical task for a company** and demonstrates clean design, separation of concerns,  
dependency inversion, exception handling, rate limiting, and unit testing using **xUnit**.

---

## ⚙️ Features
- CRUD operations for managing dogs 🐾  
- **Rate Limiting Middleware** to control request frequency  
- **Global Exception Handling Middleware**  
- **Layered structure** based on Onion Architecture  
- Database using **SQLite + Entity Framework Core**  
- Unit tests for services, middleware, and controllers (xUnit + FluentAssertions)  
- Ready-to-run via **Swagger UI**

---


The **Onion Architecture** focuses on domain-centric design and dependency inversion:  
- **Domain** — core business entities and logic (center of the system)  
- **Application** — business rules, DTOs, interfaces, and services  
- **Infrastructure** — persistence, repositories, and external dependencies  
- **API layer** — entry point with controllers and middleware  

This approach provides high testability, flexibility, and loose coupling between components.

---

## 🧪 Testing
All unit tests are located in:
DogsHouseService.Tests/

diff
Копіювати код
They cover:
- Business logic in services  
- Middleware (Rate Limiting, Exception Handling)  
- Controller responses  

Run tests using:

dotnet test
🚀 How to Run Locally
Make sure you have .NET SDK 9.0+ installed.

1️⃣ Clone the repository:
git clone https://github.com/SqeeZarion/TaskForCompany.git

2️⃣ Navigate to the project:
cd TaskForCompany/DogsHouseService

3️⃣ Run the API:
dotnet run

4️⃣ Open Swagger UI/Postman:
https://localhost:5001/swagger
https://localhost:7159/swagger

---

👤 Author
Developed by SqeeZarion (Yura)
as part of a technical assignment and portfolio backend project.
