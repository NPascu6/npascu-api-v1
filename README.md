# npascu-api-v1

Welcome to the `npascu-api-v1` project! This is a .NET Core API designed to provide services for your application. Below, you will find information on how to set up, configure, and use this API.

## Table of Contents

- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Database Setup](#database-setup)
- [Services](#services)
- [Authentication](#authentication)
- [Swagger Documentation](#swagger-documentation)

## Getting Started

To get started with the `npascu-api-v1` project, follow these steps:

1. Clone this repository to your local machine:

   ```bash
   git clone https://github.com/yourusername/npascu-api-v1.git


# npascu_api_v1 AutoMapper Mapping Profiles

This is a C# code for AutoMapper's `MappingProfiles` class that defines mapping profiles for DTOs (Data Transfer Objects) and entity objects used in the `npascu_api_v1` project. AutoMapper simplifies object-to-object mapping and is commonly used to transform data between different representations.

## Introduction

The `MappingProfiles` class is responsible for defining AutoMapper mapping profiles for various data transfer objects (DTOs) and entity objects within the `npascu_api_v1` project. It allows for seamless transformation between DTOs and entity objects.

## Prerequisites

Before using the `MappingProfiles` class, make sure you have the following components set up:

- [AutoMapper](https://www.nuget.org/packages/AutoMapper/)
- DTO classes (e.g., `ItemDto`, `OrderDto`, `UserDto`) and entity classes (e.g., `Item`, `Order`, `User`)
- AutoMapper configurations set up in your application

## Usage

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

## Mappings

The `MappingProfiles` class defines the following mappings:

- `Item` to `ItemDto` and vice versa.
- `CreateOrderDto` to `Order`.
- `Order` to `OrderDto` and vice versa.
- `CreateOrderItemDto` to `OrderItem`.
- `OrderItem` to `OrderItemDto` and vice versa.
- `CreateUserDto` to `User`.
- `User` to `UserDto` and vice versa.
- `UserItem` to `UserItemDto` and vice versa.

These mappings simplify the transformation of data between DTOs and entity objects, making it easier to work with data in your application.

## Auth Service

The `AuthService` class is responsible for handling user authentication, registration, email verification, and token generation for the `npascu_api_v1` project. It uses JWT tokens for user authentication and email verification. The class also logs various activities using a logger.

## Prerequisites

Before using the `AuthService` class, make sure you have the following components set up:

- [Microsoft.IdentityModel.Tokens](https://www.nuget.org/packages/System.IdentityModel.Tokens.Jwt/)
- .NET Core application with the necessary configurations
- An email service for sending email verifications

## Usage

1. **Constructor**:

   Create an instance of `AuthService` by providing the necessary dependencies:

   ```csharp
   var authService = new AuthService(config, authRepository, emailService, logger);
   ```

2. **User Authentication**:

   Authenticate a user by calling the `Login` method with a valid username and password:

   ```csharp
   string token = authService.Login(username, password);
   ```

   The `Login` method generates a JWT token for the authenticated user.

3. **User Registration**:

   Register a new user by calling the `Register` method with a username, email, and password:

   ```csharp
   string token = authService.Register(username, email, password);
   ```

   The `Register` method performs user registration, generates a registration token, and sends an email verification link.

4. **Email Verification**:

   Validate the email verification token by calling the `ValidateEmail` method with the token:

   ```csharp
   bool isValid = authService.ValidateEmail(token);
   ```

   The `ValidateEmail` method checks if the token is valid and marks the user's email as verified.

5. **Delete User**:

   Delete a user by calling the `DeleteUser` method with the username:

   ```csharp
   bool deleted = authService.DeleteUser(username);
   ```

   The `DeleteUser` method removes the user from the system.

## Methods

- `Login(username, password)` - Authenticates a user and returns a JWT token if successful.

- `Register(username, email, password)` - Registers a new user, generates a registration token, and sends an email verification link.

- `ValidateEmail(token)` - Validates an email verification token.

- `DeleteUser(username)` - Deletes a user from the system.

- Various private helper methods for generating JWT tokens, validating email addresses, and sending email verifications.

## License

This code is provided under the MIT License. See the [LICENSE](LICENSE) file for more details.
