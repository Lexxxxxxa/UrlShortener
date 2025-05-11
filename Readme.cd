# UrlShortener

UrlShortener is a web service for creating short links with user registration and authentication features. Users can generate short links, follow them, and view the list of previously created links.

## Technologies

* **ASP.NET Core 8** — used to create Web API and Razor Pages.
* **Entity Framework Core** — ORM for working with SQL Server database.
* **SQL Server** — relational database to store links and user data.
* **Angular 17** — frontend SPA located in the `ClientApp` folder, integrated with .NET through SPA services.
* **Bootstrap 5** — used for styling both Razor and Angular interfaces.
* **JWT/Cookie Authentication** — protects access to API endpoints and Razor pages.

## Features

* User registration and login
* Short link generation from a regular URL
* Redirecting via `ShortCode`
* Viewing all created links (for logged-in users)
* Combined Angular and Razor UI in one application

## How to Run

1. Clone the project:

   ```
   git clone https://github.com/yourusername/UrlShortener.git
   cd UrlShortener
   ```

2. Set the connection string in `appsettings.json`:

   ```
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=Your Data Source;Initial Catalog=UrlShortenerDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
  },
   ```

3. Apply migrations to create the database:

   ```
   dotnet ef database update
   ```

4. **Create the first user manually:**

   * Start the app and register a new user via the registration page.
   * **To add an admin**, insert the user directly into the `AspNetUsers` table and set the appropriate fields like `EmailConfirmed`, `IsAdmin`, etc.

5. Navigate to the frontend folder and build the Angular app:

   ```
   cd ClientApp
   npm install
   npm run build
   cd ..
   ```
6. Then run the project from Microsoft Visual Studio

---
