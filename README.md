# ASP.NET MVC Layered System

Academic enterprise-style web application built with ASP.NET MVC 5 and C#. The solution demonstrates separation between presentation, business logic, repositories, data access, models, and supporting services.

## Features

- Authentication and custom authorization filters
- Product management with CRUD operations
- Notification management with CRUD operations
- Search, pagination, modal details, and reusable partial views
- Product ranking through a JSON endpoint and dynamic client-side rendering
- Repository and business layers for data access and application logic
- Validation and exception-handling components

## Technology

- C# and .NET Framework
- ASP.NET MVC 5 and Razor
- Entity Framework / EDMX
- SQL Server
- JavaScript and jQuery
- Bootstrap
- Git and Visual Studio

## Solution structure

```text
AP.MVC           Presentation, controllers, views, and view models
AP.Core          Business rules, validation, and exceptions
AP.Repositories  Repository abstractions and data operations
AP.Data          Entity Framework model and entities
AP.Models        Shared application models
AP.Services      Supporting service integrations
```

## Run locally

1. Open `AP.sln` in Visual Studio on Windows.
2. Restore the NuGet packages referenced by the projects.
3. Review the connection strings and local database configuration.
4. Set `AP.MVC` as the startup project.
5. Build the solution and run it with IIS Express.

## Learning outcomes

- Extended an existing multi-project solution without rewriting unrelated code
- Applied MVC and layered-architecture concepts
- Implemented repository and business-layer operations
- Added filtering, pagination, AJAX partial updates, and JSON responses
- Practiced validation, authorization, exception handling, and SOLID analysis

## Scope

This repository contains university coursework and portfolio exercises. It is intended to demonstrate learning progress in C#, ASP.NET MVC, SQL Server, and software architecture.

