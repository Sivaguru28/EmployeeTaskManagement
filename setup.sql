-- =======================================================
-- 1. Table Creation Scripts (with constraints, PKs, FKs)
-- =======================================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'EmployeeTaskManagement')
BEGIN
    CREATE DATABASE EmployeeTaskManagement;
    PRINT 'Database [EmployeeTaskManagement] created.';
END
ELSE
BEGIN
    PRINT 'Database [EmployeeTaskManagement] already exists. Skipping creation.';
END
GO

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
		[IsActive] BIT NOT NULL CONSTRAINT DF_EmployeeTask_IsActive     DEFAULT 1,
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

CREATE OR ALTER PROCEDURE [dbo].[GetDashboardStats]
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


EXEC sp_rename 'dbo.Tasks', 'EmployeeTasks';




INSERT INTO dbo.Employees
(
    EmployeeCode,
    FirstName,
    LastName,
    Email,
    MobileNumber,
    Department,
    Designation,
    DateOfJoining
)
VALUES
('EMP001', 'John', 'Doe', 'john.doe@company.com', '9876543210', 'IT', 'Software Engineer', '2024-01-15'),

('EMP002', 'Jane', 'Smith', 'jane.smith@company.com', '9876543211', 'HR', 'HR Executive', '2023-11-20'),

('EMP003', 'Michael', 'Johnson', 'michael.johnson@company.com', '9876543212', 'Finance', 'Accountant', '2022-08-10'),

('EMP004', 'Emily', 'Williams', 'emily.williams@company.com', '9876543213', 'IT', 'Senior Developer', '2021-05-18'),

('EMP005', 'David', 'Brown', 'david.brown@company.com', '9876543214', 'Sales', 'Sales Manager', '2020-09-25');



INSERT INTO dbo.EmployeeTasks
(
    EmployeeId,
    Title,
    Description,
    Priority,
    Status,
    StartDate,
    DueDate,
    EstimatedHours
)
VALUES
(1,
'Develop Login API',
'Create JWT authentication and login endpoint.',
'High',
'In Progress',
'2026-07-28',
'2026-07-31',
16),

(2,
'Conduct Employee Interview',
'Schedule and complete technical interviews.',
'Medium',
'Pending',
'2026-07-29',
'2026-08-01',
8),

(3,
'Prepare Financial Report',
'Generate monthly finance report.',
'High',
'Completed',
'2026-07-20',
'2026-07-25',
20),

(4,
'Implement Task Module',
'Develop CRUD APIs for Employee Tasks.',
'High',
'In Progress',
'2026-07-27',
'2026-08-02',
24),

(5,
'Client Presentation',
'Prepare project presentation for client meeting.',
'Low',
'Pending',
'2026-07-30',
'2026-08-03',
6),

(1,
'Bug Fixing',
'Resolve production issues reported by QA.',
'Medium',
'Pending',
'2026-07-28',
'2026-07-30',
10),

(4,
'Code Review',
'Review pull requests from junior developers.',
'Low',
'Completed',
'2026-07-22',
'2026-07-22',
4);