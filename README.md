How to Run the Application Locally

Requirements:

-Visual Studio 2022 (or newer)

-.NET SDK installed

-SQL Server (LocalDB or any SQL Server instance)

Step 1: Clone the repository
git clone https://github.com/Pzydusk/KingPriceAssessment.git

Or do it through Visual Studio

Step 2: Configure the database connection
UserManagement.API/appsettings.json

Update the connection string to match your local SQL Server.

Example

"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=UserManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
}

Step 3: Create the database

In Visual Studio, open Package Manager Console and run:

Update-Database

Step 4: Run the API

Set UserManagement.API as the startup project and run it.
Swagger will open in the browser in development mode so the API endpoints can be tested.

Step 5: Run the Web App

Set UserManagement.Web as the startup project and run it.
The web application will connect to the API and allow you to manage users through the UI.

How to Run and Test the Application Locally (Unit Tests)

Open the solution in Visual Studio

From the top menu, go to: Test → Test Explorer

Click Run All Tests

Visual Studio will build the solution and run all tests in UserManagement.Tests.
Passed tests will show green ticks and failed tests will show red crosses.
