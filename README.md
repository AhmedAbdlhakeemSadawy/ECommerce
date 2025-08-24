# ECommerce

# 🛒 E-Commerce Application (.NET Core)

This is a self-study project for building a full-featured eCommerce application using **.NET Core** and **Angular** for front end.  
The project demonstrates clean architecture .


---

## 🚀 Features
- choose one scenario from business to demonstrates clean architecture concepts "Add order"
- The application architecture follows a layered design consisting of Business, Data Access, Infrastructure, and Web API layers. Each layer is divided into two projects: an Abstraction project that defines contracts and an Implementation project that provides the concrete logic. All implementation projects depend only on the corresponding abstraction projects. For example, the Infrastructure layer relies on the Business Abstraction project.
- Each Layer has DTO project contain DTOs to path data betqeen layers for exmple DataAccessDTo contain DTOS used in DataAccess Layer and each layer need to path data to access layer use these dtos
- User authentication & authorization (ASP.NET Core Identity + JWT) and apply caching for refresh token using two implementations (Redis & In Memory Cache)
- Using Entity Framework Core with repository pattern and unit of work to handle database operations
- using xunit for unit test for business functions "functions in Business Layer"
- Using Azure communication service to send email notifications
- The application applies an event-driven design when sending notifications. This separates the order creation logic from the notification logic by using an in-memory EventBus.


---
## 🏗️ Project Structure

+---Business <br>
&emsp;&emsp;|   +---ECommerceBuinessDTO <br>
&emsp;&emsp;|   +---ECommerceBusinessAbstractions <br>
&emsp;&emsp;|   +---ECommerceBusinessLogic <br>
&emsp;&emsp;|   +---ECommerceBusinessTests <br>
&emsp;&emsp;|   +---ECommerceEvents <br>
+---DataAccess <br>
&emsp;&emsp;|   +---ECommerceDataAccessDTO <br>
&emsp;&emsp;|   +---ECommerceDataAccessAbstractions <br>
&emsp;&emsp;|   +---ECommerceDataAccess <br>
+---Infrastructure <br>
&emsp;&emsp;|   +---ECommerceInfrastructureAbstractions <br>
&emsp;&emsp;|   +---ECommerceInfrastructure <br>
+---WebAPI <br>
&emsp;&emsp;|   +---WebApiAbstraction <br>
&emsp;&emsp;|   +---ECommwerceWebAPI <br>
&emsp;&emsp;|   +---ECommwerceWebAPIDto

---

## ⚙️ Tech Stack
- **.NET 8** (or your version)
- **Entity Framework Core**
- **SQL Server**
- **ASP.NET Core Identity**
- **AutoMapper**
- **Swagger** (API documentation)
- **Azure Communication Service** (Send Notification)

