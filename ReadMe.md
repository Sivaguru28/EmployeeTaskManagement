# Employee Task Management System

An enterprise-grade Full Stack Employee Task Management System built using **ASP.NET Core 8 Web API**, **Angular (v17)**, and **SQL Server**.

---

## Technical Features & Stack

*   **Backend**: ASP.NET Core 8 Web API, Entity Framework Core (EF Core), SQL Server.
*   **Frontend**: Angular v17 (Standalone Components), Bootstrap 5, PrimeNG 17 (Light-indigo theme), PrimeIcons.
*   **Security**: JWT Bearer Authentication, CORS Origin policies.
*   **Stored Procedures**: Custom database stored procedure for dashboard metric calculations.
*   **Exception Handling**: Custom Global exception middleware mapping server errors cleanly.
*   **Logging**: Serilog file-rolling and console enrichment for CRUD actions and authentication logs.

---

## Project Structure

```
EmployeeTaskManagement/
│
├── setup.sql                               # Complete database setup (Schema, SPs, Queries)
├── ReadMe.md                               # Setup and documentation instructions
│
├── Service/                                # Backend Project Folder
│   └── EmployeeTaskManagement.API/         # ASP.NET Core Web API Project
│
└── Portal/                                 # Frontend Project Folder
    ├── src/
    │   ├── app/                            # Components, Services, Guards, and Interceptors
    │   └── assets/                         # Graphic illustrations and converted assets
    └── package.json
```

---

## Local Setup & Execution Guide

### 1. Database Setup
1. Open SQL Server Management Studio (SSMS) or command line and connect to your local **SQL Express** or **SQL Server** instance.
2. Run the [setup.sql](file:///d:/EmployeeTaskManagement/setup.sql) script file to create the tables, define foreign key relations, constraints, and compile the `GetDashboardStats` Stored Procedure.
3. If necessary, check `DefaultConnection` in [appsettings.json](file:///d:/EmployeeTaskManagement/Service/EmployeeTaskManagement.API/appsettings.json) and verify that it matches your local instance configuration:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=EmployeeTaskManagement;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

### 2. Run Backend Web API
1. Navigate to the backend directory:
   ```bash
   cd Service/EmployeeTaskManagement.API
   ```
2. Build and run the project:
   ```bash
   dotnet run --launch-profile http
   ```
3. The API will start listening on [http://localhost:7141/](http://localhost:7141/).
4. Access Swagger documentation at: [http://localhost:7141/swagger/index.html](http://localhost:7141/swagger/index.html).

### 3. Run Frontend Portal
1. Navigate to the frontend directory:
   ```bash
   cd Portal
   ```
2. Install dependencies (if not already cached):
   ```bash
   npm install
   ```
3. Run the development server:
   ```bash
   npm start
   ```
4. Access the portal in your web browser at: [http://localhost:4200/](http://localhost:4200/).

---

## Seeded Admin Credentials

To log into the system, use the following credentials on the login screen:
*   **Username**: `admin`
*   **Password**: `Admin@123`

---

## Required SQL Queries (Deliverables)

The SQL queries requested in the guidelines are documented at the bottom of [setup.sql](file:///d:/EmployeeTaskManagement/setup.sql):

1.  **Query 1: List all employees with their total count of assigned tasks**:
    ```sql
    SELECT e.EmployeeId, e.EmployeeCode, e.FirstName, e.LastName, COUNT(t.EmployeeTaskId) AS TotalAssignedTasks
    FROM Employees e
    LEFT JOIN EmployeeTasks t ON e.EmployeeId = t.EmployeeId
    GROUP BY e.EmployeeId, e.EmployeeCode, e.FirstName, e.LastName;
    ```
2.  **Query 2: Find all overdue tasks (DueDate passed and Status is not Completed)**:
    ```sql
    SELECT * FROM EmployeeTasks
    WHERE DueDate < SYSUTCDATETIME() AND Status <> 'Completed';
    ```
3.  **Query 3: Find department-wise employee count**:
    ```sql
    SELECT Department, COUNT(*) AS EmployeeCount
    FROM Employees
    GROUP BY Department;
    ```
4.  **Query 4: Find the top 3 employees with the most completed tasks**:
    ```sql
    SELECT TOP 3 e.EmployeeId, e.EmployeeCode, e.FirstName, e.LastName, COUNT(t.EmployeeTaskId) AS CompletedTasksCount
    FROM Employees e
    INNER JOIN EmployeeTasks t ON e.EmployeeId = t.EmployeeId
    WHERE t.Status = 'Completed'
    GROUP BY e.EmployeeId, e.EmployeeCode, e.FirstName, e.LastName
    ORDER BY CompletedTasksCount DESC;
    ```

---

## Implemented Bonus Tasks

We selected and developed the following **two** bonus requirements:
1.  **Swagger UI Documentation**: Configured complete endpoint visibility with JWT authentication padlock integration so secure endpoints can be authorized and tested directly inside the browser.
2.  **Mobile-Responsive UI Polish**: Overhauled the frontend with fluid grids and media queries. Implemented a hover-expanding narrow menu bar on desktops (`70px` expanding to `240px`) and an overlay navigation menu drawer sliding in from the left on mobile layout sizes.

---

## Key Development Assumptions

1.  **Authentication Scope**: Since full user account management was not required, a single seeded `admin` account is hardcoded on the backend. Authentication is validated using symmetric security keys emitting standard JSON Web Tokens expiring in 60 minutes.
2.  **Soft Delete Behavior**: In accordance with enterprise specifications, deleting an employee is implemented as a soft delete (updating `IsActive` to `false`). Soft-deleted employees are filtered during standard lists and drop downs, and tasks assigned to them remain intact. Task deletes are hard deletes.