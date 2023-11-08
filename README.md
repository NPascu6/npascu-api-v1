# npascu-api-v1

# npascu_api_v1 AuthService

This is a C# code for an `AuthService` class that provides authentication and user registration functionality for the `npascu_api_v1` project. The class uses `Microsoft.IdentityModel.Tokens` for generating JWT tokens and implements various authentication and registration methods.

## Table of Contents

- [Introduction](#introduction)
- [Prerequisites](#prerequisites)
- [Usage](#usage)
- [Methods](#methods)
- [License](#license)

## Introduction

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
