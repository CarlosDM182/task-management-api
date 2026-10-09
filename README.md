# Task Management API

REST API for task management built with ASP.NET Core and .NET 10.

## Technologies

* .NET 10
* ASP.NET Core Web API
* C#
* Dapper
* SQL Server
* JWT Authentication
* BCrypt
* xUnit
* Moq
* OpenAPI
* Docker
* GitHub Actions

## Architecture

The project follows a layered architecture inspired by Clean Architecture.

TaskManagement
│
├── TaskManagement.Api
│   ├── Controllers
│   ├── Configuration
│   └── Middleware
│
├── TaskManagement.Application
│   ├── DTOs
│   ├── Interfaces
│   └── Services
│
├── TaskManagement.Domain
│   ├── Entities
│   └── Enums
│
├── TaskManagement.Infrastructure
│   ├── Persistence
│   ├── Repositories
│   └── Security
│
├── TaskManagement.Tests
│
└── Database
└── SQL scripts



## Features

* User registration
* User login
* JWT authentication
* BCrypt password hashing
* Task CRUD operations
* User ownership validation
* Protected endpoints
* SQL Server persistence
* Dapper data access
* Global exception handling
* Input validation
* Duplicate email validation
* HTTP status code handling
* OpenAPI documentation

## Authentication and Authorization

The API uses JWT Bearer authentication.<br>
After logging in, the API returns a JWT token that must be included in protected requests: Authorization: Bearer {token}

Users can only access, update, and delete their own tasks.<br>
Ownership is validated using the authenticated user's ID obtained from the JWT claims.

## API Endpoints

### Authentication

#### Login

POST /api/auth/login
Example request:
{
"email": "user@example.com",
"password": "password123"
}

## Users

### Register

POST /api/users

Example request:
{
"name": "Carlos",
"email": "carlos@example.com",
"password": "password123"
}

Get current user
GET /api/users/{id}

Get current user's tasks
GET /api/users/{id}/tasks

### Tasks

Get authenticated user's tasks
GET /api/tasks

Get task by ID
GET /api/tasks/{id}

Create task
POST /api/tasks

Example request:
{
"title": "Learn Dapper",
"description": "Create a task management API",
"dueDate": "2026-10-15T18:00:00",
"priority": "High"
}

The user ID is obtained from the authenticated JWT and is not provided by the client.
Update task
PUT /api/tasks/{id}

Example request:
{
"title": "Learn Dapper and SQL Server",
"description": "Continue improving the API",
"dueDate": "2026-10-20T18:00:00",
"priority": "High",
"status": "InProgress"
}

Delete task
DELETE /api/tasks/{id}

## HTTP Responses
* The API uses standard HTTP status codes.
* Status Code	Description
* 200	Successful request
* 201	Resource successfully created
* 204	Resource successfully updated or deleted
* 400	Invalid request
* 401	Authentication required or invalid credentials
* 403	User does not have permission to access the resource
* 404	Resource not found
* 409	Resource conflict, such as duplicate email
* 500	Unexpected server error



## Database
* The project uses SQL Server as its persistence layer.
* Database scripts are versioned in the repository:
* 001\CreateDatabase.sql
* 002\CreateTasks.sql
* 003\CreateUsers.sql
* 004\AddTaskUserForeignKey.sql
* 005\AddUserPassword.sql

Run the scripts in the order shown above.

### Database Relationship

Users
│
│ 1
│
│ N
▼
Tasks

Each task belongs to a user through the UserId foreign key.

## SQL-First Approach
This project uses a SQL-first approach with Dapper instead of Entity Framework Core.<br>
SQL Server is treated as the source of truth for the database schema.<br>
Dapper is responsible for executing SQL queries and mapping database results to domain entities.<br>
This approach provides explicit control over:

* SQL queries
* Database schema
* Relationships
* Performance
* Database changes

SQL scripts are versioned together with the application code.

## Configuration
Configure the SQL Server connection string in:
TaskManagement.Api/appsettings.json

Example:
{
"ConnectionStrings": {
"TaskManagementDb": "Server=localhost\\SQLEXPRESS;Database=TaskManagementDb;Trusted\_Connection=True;TrustServerCertificate=True;"
}
}

Configure JWT settings:
{
"Jwt": {
"Key": "your-development-key",
"Issuer": "TaskManagement.Api",
"Audience": "TaskManagement.Client",
"ExpirationMinutes": 60
}
}

Do not use development secrets in production.

For production environments, JWT keys and other sensitive configuration values should be stored using secure configuration mechanisms such as environment variables or cloud secret management.

## Running the Project

### Restore dependencies

dotnet restore

### Build the solution

dotnet build

### Run the API

dotnet run --project TaskManagement.Api

### Run tests

dotnet test

## OpenAPI
The API exposes an OpenAPI document during development.
/openapi/v1.json

## Testing
The project includes unit tests using xUnit and Moq.<br>
The tests cover important application and business rules, including:

* Task creation
* Task creation with an invalid user
* Authenticated user ownership
* Task retrieval
* Task retrieval by ownership
* Task updates
* Task update ownership
* Task deletion
* Task deletion ownership
* User creation
* Duplicate email validation

Run all tests with:
dotnet test

## Project Structure

### TaskManagement.Api

Responsible for the HTTP layer of the application.<br>
It contains:

* Controllers
* JWT authentication configuration
* Middleware
* API configuration
* HTTP request handling

### TaskManagement.Application

Contains the application's business use cases.<br>
It contains:

* DTOs
* Application interfaces
* Application services
* Business flow orchestration

### TaskManagement.Domain

Contains the core domain model.<br  >
It contains:

* Entities
* Enumerations
* Domain concepts
The Domain layer does not depend on Infrastructure or API.

### TaskManagement.Infrastructure

Contains implementations related to external concerns.<br>
It contains:

* SQL Server connection management
* Dapper repositories
* Password hashing
* JWT token generation
* Infrastructure dependency injection

### TaskManagement.Tests

Contains unit tests for application services and business rules.<br>
The project uses:

* xUnit
* Moq

## Development Principles
The project applies several software engineering principles and patterns:

* SOLID principles
* Dependency Injection
* Repository Pattern
* Layered Architecture
* Separation of Responsibilities
* DTO pattern
* Unit Testing
* SQL-first development

## Error Handling
The API includes global exception handling through custom middleware.<br>
Known application errors are converted into appropriate HTTP responses.

Unexpected exceptions are logged and returned as:
500 Internal Server Error

without exposing internal implementation details to the client.

## Security
Security-related practices implemented in the project include:

* JWT Bearer authentication
* BCrypt password hashing
* Passwords are never stored as plain text
* Ownership validation for task operations
* Parameterized SQL queries
* Protected API endpoints
* Duplicate email validation

## Continuous Integration and Containerization

The project includes automated CI using GitHub Actions.

The CI pipeline:

* Restores dependencies
* Builds the solution in Release configuration
* Runs the unit tests

The application is also containerized using Docker with a multi-stage build.


## Docker

* Build the Docker image:

* docker build -t taskmanagement-api .

* Run the container:

* docker run --name taskmanagement-api-container -p 8080:8080 taskmanagement-api

For containerized environments, sensitive configuration such as the database connection string and JWT settings should be provided through environment variables.

## Future Improvements

Planned improvements for this project include:

* Cloud deployment
* Application monitoring
* Production configuration and secret management
* Automated deployment
