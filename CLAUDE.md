# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an ASP.NET Core 8.0 Web API project for customer management. It's a technical assessment project that implements a complete customer management API with address lookup via the ViaCEP external service.

## Development Commands

```bash
# Build the project
dotnet build

# Run the application (defaults to https://localhost:7257 or http://localhost:5005)
# IMPORTANT: Always open the .sln file in Visual Studio for best experience
dotnet run

# Run in watch mode for development
dotnet watch run

# Restore NuGet packages
dotnet restore

# Run tests (when implemented)
dotnet test
```

## Architecture Requirements

The project needs to implement the following architectural patterns:

### Repository + Service Pattern
- Create repository interfaces and implementations for data access
- Create service interfaces and implementations for business logic
- Use dependency injection to wire everything together in `Program.cs`

### Entity Framework Core Setup
- Configure with InMemory database or local SQL Server
- DbContext should be created for managing Customer entities
- All database operations must be asynchronous

### HTTP Client Configuration
- Configure `HttpClient` via `IHttpClientFactory` in `Program.cs`
- Used for consuming ViaCEP API: `https://viacep.com.br/ws/{cep}/json/`

### Global Error Handling
- Implement middleware for global exception handling
- Return appropriate HTTP status codes with meaningful error messages

## Key Implementation Points

### Customer Entity Structure
```csharp
public class Customer
{
    public long Id { get; set; }
    public string Name { get; set; }  // Required
    public string Email { get; set; } // Required, valid format
    public string Cep { get; set; }   // Required, digits only
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
}
```

### Required Endpoints in CustomersController
- `GET /api/customers` - List all customers
- `GET /api/customers/{id}` - Get customer by ID
- `POST /api/customers` - Create new customer
- `PUT /api/customers/{id}` - Update existing customer
- `DELETE /api/customers/{id}` - Delete customer

### ViaCEP Integration
When creating or updating a customer:
1. Validate CEP format (digits only)
2. Call ViaCEP API before saving
3. Populate Street, City, and State from response
4. Return 400 Bad Request if CEP is invalid or service fails

## Current Project State

The project is currently a template with:
- Basic Web API configuration with Swagger
- Empty `CostumersController` (note the typo in the filename - should be `CustomersController`)
- No models, services, or repositories implemented yet
- No Entity Framework configuration
- No HTTP client configuration for ViaCEP