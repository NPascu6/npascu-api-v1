# Use the official .NET 8 SDK as a build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy and restore dependencies
COPY . ./
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o /out

# Use a smaller runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Expose port (default ASP.NET Core port)
EXPOSE 8080

# Set entrypoint
ENTRYPOINT ["dotnet", "npascu-api-v1.dll"]
