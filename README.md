# npascu-api-v1

Welcome to the `npascu-api-v1` project! This is a .NET Core API designed to provide data for your web application.
Below, you will find comprehensive information on how to set up, configure, and use this API effectively.

##
JWT Authentication / Authorization
Registration + password hashing (explicit user control).
Email Verification / Deletion if not verified in 24h.
Basic Item, Order and User DB relation.

## Table of Contents

- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Database Setup](#database-setup)
- [Services](#services)
- [Authentication](#authentication)
- [Swagger Documentation](#swagger-documentation)
- [AutoMapper Mapping Profiles](#automapper-mapping-profiles)

## Getting Started

To get started with the `npascu-api-v1` project, follow these steps:

1. Clone this repository to your local machine:

   ```bash
   git clone https://github.com/yourusername/npascu-api-v1.git
   ```

2. Open the project in your favorite development environment (e.g., Visual Studio or Visual Studio Code).

3. Restore the necessary packages using NuGet:

   ```bash
   dotnet restore
   ```

4. Configure the application as described in the [Configuration](#configuration) section.

5. Set up your database by following the [Database Setup](#database-setup) instructions.

6. Run the application:

   ```bash
   dotnet run
   ```

7. Your API should be up and running. You can access it at `http://localhost:5000` or another port, depending on your configuration.

## Configuration

The project's configuration is managed through the `appsettings.json` file, which contains settings for your application, including the database connection, authentication, and more. Ensure that you have the appropriate configuration values set before running the application.

## Database Setup

The `npascu-api-v1` project uses Entity Framework Core to manage its database. You can set up your database by running Entity Framework migrations. To do this, run the following commands:

```bash
dotnet ef migrations add InitialMigration
dotnet ef database update
```

This will create the necessary database tables based on your models.

## Services

This project provides various services that can be used within your application. Some of the services available include user management, item management, order management, and email services. These services are designed to be easily integrated into your application logic.

To use a service, simply inject the corresponding interface into your classes and make use of the provided methods.

## Authentication

The `npascu-api-v1` project includes JWT (JSON Web Token) authentication for secure access to your API endpoints. You can configure JWT authentication by updating the `appsettings.json` file with your desired settings.

Please note that you should secure your application by using strong secret keys and following security best practices.

## Swagger Documentation

This project is configured with Swagger for easy API documentation. After running the application, you can access the Swagger documentation at `http://localhost:5000/swagger`. Swagger provides interactive documentation for your API endpoints, making it easier for developers to understand and use your API.

## AutoMapper Mapping Profiles

### Introduction

The `MappingProfiles` class is responsible for defining AutoMapper mapping profiles for various data transfer objects (DTOs) and entity objects used in the `npascu_api_v1` project. AutoMapper simplifies object-to-object mapping and is commonly used to transform data between different representations.

### Prerequisites

Before using the `MappingProfiles` class, make sure you have the following components set up:

- [AutoMapper](https://www.nuget.org/packages/AutoMapper/)
- DTO classes (e.g., `ItemDto`, `OrderDto`, `UserDto`) and entity classes (e.g., `Item`, `Order`, `User`)
- AutoMapper configurations set up in your application

### Usage

1. **Configuration**:

   To use AutoMapper with the defined mapping profiles, ensure that AutoMapper is configured in your application. This typically involves setting up AutoMapper profiles and mappings within your application's configuration.

2. **Usage in Services**:

   In your services, you can use the AutoMapper mappings to transform data between DTOs and entity objects. For example:

   ```csharp
   // Create an instance of MappingProfiles
   var mapper = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<MappingProfiles>()).CreateMapper();

   // Map an Item entity to an ItemDto
   Item itemEntity = GetItemFromDatabase();
   ItemDto itemDto = mapper.Map<ItemDto>(itemEntity);

   // Map an OrderDto to an Order entity
   CreateOrderDto createOrderDto = GetCreateOrderDtoFromRequest();
   Order orderEntity = mapper.Map<Order>(createOrderDto);

   // Reverse mapping from Order entity to OrderDto
   Order orderEntity = GetOrderFromDatabase();
   OrderDto orderDto = mapper.Map<OrderDto>(orderEntity);
   ```

   The `MappingProfiles` class ensures that mappings are defined for various DTOs and entity objects.

### Mappings

The `MappingProfiles` class defines mappings for:

- `Item` to `ItemDto` and vice versa.
- `CreateOrderDto` to `Order`.
- `Order` to `OrderDto` and vice versa.
- `CreateOrderItemDto` to `OrderItem`.
- `OrderItem` to `OrderItemDto` and vice versa.
- `CreateUserDto` to `User`.
- `User` to `UserDto` and vice versa.
- `UserItem` to `UserItemDto` and vice versa.

These mappings simplify the transformation of data between DTOs and entity objects, making it easier to work with data in your application.

## License

This code is provided under the MIT License. See the [LICENSE](LICENSE) file for more details.

Please replace placeholders like `https://github.com/yourusername/npascu-api-v1.git` with the actual repository URL and adjust other details as needed for your specific project.
