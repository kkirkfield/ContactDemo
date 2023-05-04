# Contact Demo App

*This application is for demo purposes only and is not intended for production use.*

This application is a simple contact management application. It allows users to create, read, update, and delete contacts. It also allows users to search for contacts by name or address using fuzzy search.

## Project Structure

This repository partially uses Domain Driven Design (DDD) and is broken up into the following projects:
- `ContactDemo.WebApp` is the Presentation Layer, Application Layer, and Infrastructure Layer. This is where the ASP.NET Core MVC application lives. This layer also includes any models and services specific to this Presentation Layer that cannot be shared with other Presentation Layers.
- `ContactDemo.Domain` is the Domain Layer. This is where the domain models and business logic live.

Clean Architecture could be implemented to further improve the separation of concerns between the layers.

## Live Demo

This demo application is available [live](). You will be provided a password to access the application ahead of time.

## Local Demo

### 1. Prerequisites

- [.NET SDK 7.0.203 or greater 7.X version](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

  Choose the latest SDK installer for your OS at the link above.

- Restore .NET CLI local tools

  ```sh
  cd <repository root>
  dotnet tool restore
  ```

### 2. Update the connection string (Optional)

You can optionally update the connection string for local development at: [/src/ContactDemo.WebApp/appsettings.Development.json](/src/ContactDemo.WebApp/appsettings.Development.json)

### 3. Run the database migrations

```sh
cd src/ContactDemo.WebApp/
dotnet ef database update
```

### 4. Build and run the application

```sh
dotnet run --launch-profile https
```

### 5. Login with default local password; or change password (Optional)

The application does not require a password while running in the Development environment (i.e. when debugging). You can bypass the login screen by going to [https://localhost:5001/Contacts/Index](https://localhost:5001/Contacts/Index).

When running in a non-Development environment (i.e. Staging or Production), a pre-generated password is required to access secured pages.

The default local password is `7735785`. To change the password, run the application and go to [https://localhost:5001/Home/Hash](https://localhost:5001/Home/Hash). Enter a new password, click Generate, and copy the new hash to [/src/ContactDemo.WebApp/appsettings.Development.json](/src/ContactDemo.WebApp/appsettings.Development.json). When deploying to production, the hash is stored in the environment variable `LoginOptions__HashedPassword` instead of `appsettings.json`.

### 6. Cleanup the local database

```sh
dotnet ef database drop
```
