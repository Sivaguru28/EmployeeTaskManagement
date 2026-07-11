-- =======================================================
-- 1. Table Creation Scripts (with constraints, PKs, FKs)
-- =======================================================

-- Create Employees Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Employees]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Employees] (
        [EmployeeId] INT IDENTITY(1,1) NOT NULL,
        [EmployeeCode] NVARCHAR(50) NOT NULL,
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(256) NOT NULL,
        [MobileNumber] NVARCHAR(20) NOT NULL,
        [Department] NVARCHAR(100) NOT NULL,
        [Designation] NVARCHAR(100) NOT NULL,
        [DateOfJoining] DATETIME2(7) NOT NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_Employees_IsActive] DEFAULT (1),
        [CreatedDate] DATETIME2(7) NOT NULL CONSTRAINT [DF_Employees_CreatedDate] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ([EmployeeId] ASC),
        CONSTRAINT [UQ_Employees_Email] UNIQUE NONCLUSTERED ([Email] ASC)
    );
END
GO

-- Create EmployeeTasks Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployeeTasks]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EmployeeTasks] (
        [EmployeeTaskId] INT IDENTITY(1,1) NOT NULL,
        [EmployeeId] INT NOT NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [Priority] NVARCHAR(50) NOT NULL, -- Low, Medium, High
        [Status] NVARCHAR(50) NOT NULL, -- Pending, In Progress, Completed
        [StartDate] DATETIME2(7) NOT NULL,
        [DueDate] DATETIME2(7) NOT NULL,
        [EstimatedHours] DECIMAL(18,2) NOT NULL,
        [CreatedDate] DATETIME2(7) NOT NULL CONSTRAINT [DF_EmployeeTasks_CreatedDate] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_EmployeeTasks] PRIMARY KEY CLUSTERED ([EmployeeTaskId] ASC),
        CONSTRAINT [FK_EmployeeTasks_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([EmployeeId]) ON DELETE NO ACTION
    );
END
GO


-- =======================================================
-- 2. Stored Procedure for Dashboard Statistics
-- =======================================================
IF OBJECT_ID('dbo.GetDashboardStats', 'P') IS NOT NULL
    DROP PROCEDURE dbo.GetDashboardStats;
GO

ALTER PROCEDURE [dbo].[GetDashboardStats]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalEmployees INT;
    DECLARE @ActiveEmployees INT;
    DECLARE @PendingTasks INT;
    DECLARE @CompletedTasks INT;
    DECLARE @OverdueTasks INT;

    SELECT @TotalEmployees = COUNT(*) FROM [dbo].[Employees];

    SELECT @ActiveEmployees = COUNT(*) FROM [dbo].[Employees] WHERE [IsActive] = 1;

    SELECT @PendingTasks = COUNT(*) FROM [dbo].[EmployeeTasks] WHERE [Status] <> 'Completed' AND IsActive = 1;

    SELECT @CompletedTasks = COUNT(*) FROM [dbo].[EmployeeTasks] WHERE [Status] = 'Completed' AND IsActive = 1;

    SELECT @OverdueTasks = COUNT(*) FROM [dbo].[EmployeeTasks] 
    WHERE [DueDate] < SYSUTCDATETIME() AND [Status] <> 'Completed' AND IsActive = 1;

    SELECT 
        ISNULL(@TotalEmployees, 0) AS TotalEmployees,
        ISNULL(@ActiveEmployees, 0) AS ActiveEmployees,
        ISNULL(@PendingTasks, 0) AS PendingTasks,
        ISNULL(@CompletedTasks, 0) AS CompletedTasks,
        ISNULL(@OverdueTasks, 0) AS OverdueTasks;
END



-- =======================================================
-- 3. SQL Query Requirements
-- =======================================================

-- Query 1: List all employees along with their total number of assigned tasks.
-- SELECT e.EmployeeId, e.EmployeeCode, e.FirstName, e.LastName, COUNT(t.EmployeeTaskId) AS TotalAssignedTasks
-- FROM Employees e
-- LEFT JOIN EmployeeTasks t ON e.EmployeeId = t.EmployeeId
-- GROUP BY e.EmployeeId, e.EmployeeCode, e.FirstName, e.LastName;

-- Query 2: Find all overdue tasks (DueDate has passed and Status is not Completed).
-- SELECT * FROM EmployeeTasks
-- WHERE DueDate < SYSUTCDATETIME() AND Status <> 'Completed';

-- Query 3: Find department-wise employee count.
-- SELECT Department, COUNT(*) AS EmployeeCount
-- FROM Employees
-- GROUP BY Department;

-- Query 4: Find the top 3 employees with the most completed tasks.
-- SELECT TOP 3 e.EmployeeId, e.EmployeeCode, e.FirstName, e.LastName, COUNT(t.EmployeeTaskId) AS CompletedTasksCount
-- FROM Employees e
-- INNER JOIN EmployeeTasks t ON e.EmployeeId = t.EmployeeId
-- WHERE t.Status = 'Completed'
-- GROUP BY e.EmployeeId, e.EmployeeCode, e.FirstName, e.LastName
-- ORDER BY CompletedTasksCount DESC;
