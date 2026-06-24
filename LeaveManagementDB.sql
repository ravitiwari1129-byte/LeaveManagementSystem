-- ========================================
-- CREATE DATABASE
-- ========================================
CREATE DATABASE LeaveManagementDB;
GO

USE LeaveManagementDB;
GO

-- ========================================
-- CREATE TABLES
-- ========================================

-- Departments Table
CREATE TABLE Departments (
    DepartmentId INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName NVARCHAR(100) NOT NULL,
    IsActive BIT DEFAULT 1
);
GO

-- Employees Table
CREATE TABLE Employees (
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    DepartmentId INT,
    Role NVARCHAR(50) NOT NULL,
    IsActive BIT DEFAULT 1,
    Password NVARCHAR(100) NOT NULL,
    FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId)
);
GO

-- LeaveRequests Table
CREATE TABLE LeaveRequests (
    LeaveId INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    LeaveType NVARCHAR(50) NOT NULL,
    FromDate DATE NOT NULL,
    ToDate DATE NOT NULL,
    Reason NVARCHAR(500) NOT NULL,
    Status NVARCHAR(20) DEFAULT 'Pending',
    AppliedDate DATETIME DEFAULT GETDATE(),
    ApprovedBy INT NULL,
    Remarks NVARCHAR(500) NULL,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId),
    FOREIGN KEY (ApprovedBy) REFERENCES Employees(EmployeeId)
);
GO

-- ========================================
-- INSERT SAMPLE DATA
-- ========================================

-- Insert Departments
INSERT INTO Departments (DepartmentName) VALUES 
('Human Resources'),
('Information Technology'),
('Finance'),
('Marketing'),
('Operations'),
('Sales');
GO

-- Insert Employees
INSERT INTO Employees (EmployeeName, Email, DepartmentId, Role, IsActive, Password) VALUES 
('Admin User', 'admin@leave.com', 1, 'Admin', 1, 'admin123'),
('John Smith', 'john@leave.com', 2, 'Employee', 1, 'emp123'),
('Sarah Johnson', 'sarah@leave.com', 3, 'Employee', 1, 'emp123'),
('Mike Brown', 'mike@leave.com', 2, 'Employee', 1, 'emp123'),
('Lisa Wilson', 'lisa@leave.com', 4, 'Employee', 1, 'emp123'),
('Tim David', 'tim@leave.com', 4, 'Employee', 0, 'emp123'),
('Travis Head', 'travis@leave.com', 2, 'Employee', 1, 'emp123'),
('Michael Clarke', 'michael@leave.com', 3, 'Employee', 1, 'emp123'),
('Shane Joe', 'shane@leave.com', 2, 'Employee', 1, 'emp123'),
('Lisal Watson', 'lisal@leave.com', 4, 'Employee', 1, 'emp123');
GO

-- ========================================
-- INSERT LEAVE REQUESTS
-- ========================================

-- For Employee 1: Admin User (admin@leave.com)
INSERT INTO LeaveRequests (EmployeeId, LeaveType, FromDate, ToDate, Reason, Status, AppliedDate, ApprovedBy, Remarks) VALUES
(1, 'Vacation', DATEADD(DAY, 5, GETDATE()), DATEADD(DAY, 10, GETDATE()), 'Annual vacation with family', 'Pending', GETDATE(), NULL, NULL),
(1, 'Sick Leave', DATEADD(DAY, -15, GETDATE()), DATEADD(DAY, -14, GETDATE()), 'Flu and fever', 'Approved', DATEADD(DAY, -20, GETDATE()), 1, 'Approved - Recovery confirmed'),
(1, 'Personal Leave', DATEADD(DAY, -30, GETDATE()), DATEADD(DAY, -30, GETDATE()), 'Personal emergency', 'Approved', DATEADD(DAY, -35, GETDATE()), 1, 'Approved'),
(1, 'Bereavement Leave', DATEADD(DAY, -60, GETDATE()), DATEADD(DAY, -58, GETDATE()), 'Family funeral', 'Approved', DATEADD(DAY, -65, GETDATE()), 1, 'Approved - Condolences');

-- For Employee 2: John Smith (john@leave.com)
INSERT INTO LeaveRequests (EmployeeId, LeaveType, FromDate, ToDate, Reason, Status, AppliedDate, ApprovedBy, Remarks) VALUES
(2, 'Sick Leave', DATEADD(DAY, 2, GETDATE()), DATEADD(DAY, 3, GETDATE()), 'High fever and body pain', 'Pending', GETDATE(), NULL, NULL),
(2, 'Vacation', DATEADD(DAY, 15, GETDATE()), DATEADD(DAY, 20, GETDATE()), 'Going to Goa with friends', 'Pending', GETDATE(), NULL, NULL),
(2, 'Personal Leave', DATEADD(DAY, -10, GETDATE()), DATEADD(DAY, -10, GETDATE()), 'Bank work', 'Approved', DATEADD(DAY, -15, GETDATE()), 1, 'Approved'),
(2, 'Sick Leave', DATEADD(DAY, -40, GETDATE()), DATEADD(DAY, -38, GETDATE()), 'COVID-19 positive', 'Approved', DATEADD(DAY, -45, GETDATE()), 1, 'Approved - Provided report'),
(2, 'Vacation', DATEADD(DAY, -90, GETDATE()), DATEADD(DAY, -85, GETDATE()), 'Summer vacation', 'Approved', DATEADD(DAY, -100, GETDATE()), 1, 'Approved'),
(2, 'Personal Leave', DATEADD(DAY, -5, GETDATE()), DATEADD(DAY, -4, GETDATE()), 'Urgent personal work', 'Rejected', DATEADD(DAY, -8, GETDATE()), 1, 'Rejected - Team workload high');

-- For Employee 3: Sarah Johnson (sarah@leave.com)
INSERT INTO LeaveRequests (EmployeeId, LeaveType, FromDate, ToDate, Reason, Status, AppliedDate, ApprovedBy, Remarks) VALUES
(3, 'Maternity Leave', DATEADD(DAY, 30, GETDATE()), DATEADD(DAY, 120, GETDATE()), 'Maternity leave for newborn', 'Pending', GETDATE(), NULL, NULL),
(3, 'Sick Leave', DATEADD(DAY, -8, GETDATE()), DATEADD(DAY, -7, GETDATE()), 'Stomach infection', 'Approved', DATEADD(DAY, -12, GETDATE()), 1, 'Approved - Take rest'),
(3, 'Personal Leave', DATEADD(DAY, -25, GETDATE()), DATEADD(DAY, -25, GETDATE()), 'House shifting', 'Approved', DATEADD(DAY, -30, GETDATE()), 1, 'Approved'),
(3, 'Vacation', DATEADD(DAY, -70, GETDATE()), DATEADD(DAY, -65, GETDATE()), 'Trip to Manali', 'Approved', DATEADD(DAY, -80, GETDATE()), 1, 'Approved'),
(3, 'Sick Leave', DATEADD(DAY, -3, GETDATE()), DATEADD(DAY, -2, GETDATE()), 'Dengue fever', 'Rejected', DATEADD(DAY, -6, GETDATE()), 1, 'Rejected - Need medical certificate');

-- For Employee 4: Mike Brown (mike@leave.com)
INSERT INTO LeaveRequests (EmployeeId, LeaveType, FromDate, ToDate, Reason, Status, AppliedDate, ApprovedBy, Remarks) VALUES
(4, 'Vacation', DATEADD(DAY, 10, GETDATE()), DATEADD(DAY, 17, GETDATE()), 'Europe tour with family', 'Pending', GETDATE(), NULL, NULL),
(4, 'Sick Leave', DATEADD(DAY, -12, GETDATE()), DATEADD(DAY, -11, GETDATE()), 'Migraine', 'Approved', DATEADD(DAY, -15, GETDATE()), 1, 'Approved'),
(4, 'Personal Leave', DATEADD(DAY, -20, GETDATE()), DATEADD(DAY, -19, GETDATE()), 'Marriage function', 'Approved', DATEADD(DAY, -25, GETDATE()), 1, 'Approved - Enjoy'),
(4, 'Bereavement Leave', DATEADD(DAY, -50, GETDATE()), DATEADD(DAY, -49, GETDATE()), 'Grandfather expired', 'Approved', DATEADD(DAY, -55, GETDATE()), 1, 'Approved - Condolences'),
(4, 'Sick Leave', DATEADD(DAY, -1, GETDATE()), DATEADD(DAY, -1, GETDATE()), 'Cold and cough', 'Rejected', DATEADD(DAY, -3, GETDATE()), 1, 'Rejected - Already many leaves');

-- For Employee 5: Lisa Wilson (lisa@leave.com)
INSERT INTO LeaveRequests (EmployeeId, LeaveType, FromDate, ToDate, Reason, Status, AppliedDate, ApprovedBy, Remarks) VALUES
(5, 'Personal Leave', DATEADD(DAY, 3, GETDATE()), DATEADD(DAY, 4, GETDATE()), 'Dental surgery', 'Pending', GETDATE(), NULL, NULL),
(5, 'Vacation', DATEADD(DAY, 25, GETDATE()), DATEADD(DAY, 30, GETDATE()), 'Kerala backwaters trip', 'Pending', GETDATE(), NULL, NULL),
(5, 'Sick Leave', DATEADD(DAY, -18, GETDATE()), DATEADD(DAY, -17, GETDATE()), 'Food poisoning', 'Approved', DATEADD(DAY, -22, GETDATE()), 1, 'Approved'),
(5, 'Vacation', DATEADD(DAY, -55, GETDATE()), DATEADD(DAY, -50, GETDATE()), 'Singapore trip', 'Approved', DATEADD(DAY, -65, GETDATE()), 1, 'Approved'),
(5, 'Personal Leave', DATEADD(DAY, -7, GETDATE()), DATEADD(DAY, -6, GETDATE()), 'Urgent work at home', 'Rejected', DATEADD(DAY, -10, GETDATE()), 1, 'Rejected - No prior intimation');

-- For Employee 6: Tim David (tim@leave.com) - Inactive
INSERT INTO LeaveRequests (EmployeeId, LeaveType, FromDate, ToDate, Reason, Status, AppliedDate, ApprovedBy, Remarks) VALUES
(6, 'Sick Leave', DATEADD(DAY, -30, GETDATE()), DATEADD(DAY, -28, GETDATE()), 'Viral infection', 'Approved', DATEADD(DAY, -35, GETDATE()), 1, 'Approved'),
(6, 'Personal Leave', DATEADD(DAY, -45, GETDATE()), DATEADD(DAY, -45, GETDATE()), 'Personal work', 'Approved', DATEADD(DAY, -50, GETDATE()), 1, 'Approved'),
(6, 'Vacation', DATEADD(DAY, -80, GETDATE()), DATEADD(DAY, -75, GETDATE()), 'Weekend getaway', 'Approved', DATEADD(DAY, -90, GETDATE()), 1, 'Approved');

-- For Employee 7: Travis Head (travis@leave.com)
INSERT INTO LeaveRequests (EmployeeId, LeaveType, FromDate, ToDate, Reason, Status, AppliedDate, ApprovedBy, Remarks) VALUES
(7, 'Sick Leave', DATEADD(DAY, 1, GETDATE()), DATEADD(DAY, 3, GETDATE()), 'Chicken pox', 'Pending', GETDATE(), NULL, NULL),
(7, 'Vacation', DATEADD(DAY, 40, GETDATE()), DATEADD(DAY, 45, GETDATE()), 'Thailand trip', 'Pending', GETDATE(), NULL, NULL),
(7, 'Personal Leave', DATEADD(DAY, -22, GETDATE()), DATEADD(DAY, -20, GETDATE()), 'Family function', 'Approved', DATEADD(DAY, -28, GETDATE()), 1, 'Approved'),
(7, 'Sick Leave', DATEADD(DAY, -60, GETDATE()), DATEADD(DAY, -59, GETDATE()), 'Eye infection', 'Approved', DATEADD(DAY, -65, GETDATE()), 1, 'Approved'),
(7, 'Bereavement Leave', DATEADD(DAY, -100, GETDATE()), DATEADD(DAY, -99, GETDATE()), 'Uncle expired', 'Approved', DATEADD(DAY, -105, GETDATE()), 1, 'Approved');

-- For Employee 8: Michael Clarke (michael@leave.com)
INSERT INTO LeaveRequests (EmployeeId, LeaveType, FromDate, ToDate, Reason, Status, AppliedDate, ApprovedBy, Remarks) VALUES
(8, 'Personal Leave', DATEADD(DAY, 8, GETDATE()), DATEADD(DAY, 9, GETDATE()), 'Doctor appointment', 'Pending', GETDATE(), NULL, NULL),
(8, 'Sick Leave', DATEADD(DAY, -14, GETDATE()), DATEADD(DAY, -13, GETDATE()), 'Back pain', 'Approved', DATEADD(DAY, -18, GETDATE()), 1, 'Approved - Physiotherapy recommended'),
(8, 'Vacation', DATEADD(DAY, -35, GETDATE()), DATEADD(DAY, -32, GETDATE()), 'Jaipur visit', 'Approved', DATEADD(DAY, -42, GETDATE()), 1, 'Approved'),
(8, 'Personal Leave', DATEADD(DAY, -2, GETDATE()), DATEADD(DAY, -2, GETDATE()), 'Urgent personal work', 'Rejected', DATEADD(DAY, -4, GETDATE()), 1, 'Rejected - Short notice');

-- For Employee 9: Shane Joe (shane@leave.com)
INSERT INTO LeaveRequests (EmployeeId, LeaveType, FromDate, ToDate, Reason, Status, AppliedDate, ApprovedBy, Remarks) VALUES
(9, 'Vacation', DATEADD(DAY, 12, GETDATE()), DATEADD(DAY, 19, GETDATE()), 'Himachal trekking', 'Pending', GETDATE(), NULL, NULL),
(9, 'Sick Leave', DATEADD(DAY, -5, GETDATE()), DATEADD(DAY, -4, GETDATE()), 'Fever and weakness', 'Approved', DATEADD(DAY, -9, GETDATE()), 1, 'Approved - Take medicine'),
(9, 'Personal Leave', DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, -27, GETDATE()), 'Sister wedding', 'Approved', DATEADD(DAY, -40, GETDATE()), 1, 'Approved - Congratulations'),
(9, 'Vacation', DATEADD(DAY, -75, GETDATE()), DATEADD(DAY, -70, GETDATE()), 'Ooty trip', 'Approved', DATEADD(DAY, -85, GETDATE()), 1, 'Approved'),
(9, 'Sick Leave', DATEADD(DAY, -1, GETDATE()), DATEADD(DAY, -1, GETDATE()), 'Headache', 'Rejected', DATEADD(DAY, -2, GETDATE()), 1, 'Rejected - Apply in advance');

-- For Employee 10: Lisal Watson (lisal@leave.com)
INSERT INTO LeaveRequests (EmployeeId, LeaveType, FromDate, ToDate, Reason, Status, AppliedDate, ApprovedBy, Remarks) VALUES
(10, 'Paternity Leave', DATEADD(DAY, 20, GETDATE()), DATEADD(DAY, 35, GETDATE()), 'New baby arrival', 'Pending', GETDATE(), NULL, NULL),
(10, 'Sick Leave', DATEADD(DAY, -9, GETDATE()), DATEADD(DAY, -8, GETDATE()), 'Allergy reaction', 'Approved', DATEADD(DAY, -13, GETDATE()), 1, 'Approved'),
(10, 'Vacation', DATEADD(DAY, -50, GETDATE()), DATEADD(DAY, -48, GETDATE()), 'Weekend trip', 'Approved', DATEADD(DAY, -55, GETDATE()), 1, 'Approved'),
(10, 'Personal Leave', DATEADD(DAY, -3, GETDATE()), DATEADD(DAY, -3, GETDATE()), 'Vehicle breakdown', 'Rejected', DATEADD(DAY, -5, GETDATE()), 1, 'Rejected - Manage differently');
GO

-- ========================================
-- STORED PROCEDURES
-- ========================================

-- USP_GetEmployees
CREATE OR ALTER PROCEDURE USP_GetEmployees
    @DepartmentId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.EmployeeId,
        e.EmployeeName,
        e.Email,
        e.DepartmentId,
        d.DepartmentName,
        e.Role,
        e.IsActive
    FROM Employees e
    INNER JOIN Departments d ON e.DepartmentId = d.DepartmentId
    WHERE (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
        AND e.IsActive = 1
    ORDER BY e.EmployeeName;
END
GO

-- USP_GetEmployeeById
CREATE OR ALTER PROCEDURE USP_GetEmployeeById
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.EmployeeId,
        e.EmployeeName,
        e.Email,
        e.DepartmentId,
        d.DepartmentName,
        e.Role,
        e.IsActive
    FROM Employees e
    INNER JOIN Departments d ON e.DepartmentId = d.DepartmentId
    WHERE e.EmployeeId = @EmployeeId;
END
GO

-- USP_InsertEmployee
CREATE OR ALTER PROCEDURE USP_InsertEmployee
    @EmployeeName NVARCHAR(100),
    @Email NVARCHAR(100),
    @DepartmentId INT,
    @Role NVARCHAR(50),
    @Password NVARCHAR(100),
    @EmployeeId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validation 1: Check if email already exists
    IF EXISTS (SELECT 1 FROM Employees WHERE Email = @Email)
    BEGIN
        SET @EmployeeId = -1;
        RETURN;
    END

    -- Validation 2: Check if department exists
    IF NOT EXISTS (SELECT 1 FROM Departments WHERE DepartmentId = @DepartmentId AND IsActive = 1)
    BEGIN
        SET @EmployeeId = -2;
        RETURN;
    END

    -- Validation 3: Check if role is valid
    IF @Role NOT IN ('Admin', 'Employee')
    BEGIN
        SET @EmployeeId = -3;
        RETURN;
    END

    -- Validation 4: Check if name is not empty
    IF LTRIM(RTRIM(@EmployeeName)) = '' OR @EmployeeName IS NULL
    BEGIN
        SET @EmployeeId = -4;
        RETURN;
    END

    -- Validation 5: Check if email is valid format
    IF @Email NOT LIKE '%_@__%.__%'
    BEGIN
        SET @EmployeeId = -5;
        RETURN;
    END

    -- Validation 6: Check if password is at least 4 characters
    IF LEN(@Password) < 4
    BEGIN
        SET @EmployeeId = -6;
        RETURN;
    END

    -- Insert new employee
    INSERT INTO Employees (EmployeeName, Email, DepartmentId, Role, Password, IsActive)
    VALUES (@EmployeeName, @Email, @DepartmentId, @Role, @Password, 1);

    SET @EmployeeId = SCOPE_IDENTITY();
END
GO

-- USP_CheckEmailExists
CREATE OR ALTER PROCEDURE USP_CheckEmailExists
    @Email NVARCHAR(100),
    @Exists BIT OUTPUT  
AS
BEGIN
    SET NOCOUNT ON;
    
    SET @Exists = 0;
    
    IF EXISTS (SELECT 1 FROM Employees WHERE Email = @Email)
    BEGIN
        SET @Exists = 1;
    END
END
GO

-- USP_UpdateEmployee
CREATE OR ALTER PROCEDURE USP_UpdateEmployee
    @EmployeeId INT,
    @EmployeeName NVARCHAR(100),
    @Email NVARCHAR(100),
    @DepartmentId INT,
    @Role NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM Employees WHERE EmployeeId = @EmployeeId)
    BEGIN
        SELECT 0 AS Result, 'Employee not found' AS Message;
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Employees WHERE Email = @Email AND EmployeeId != @EmployeeId)
    BEGIN
        SELECT 0 AS Result, 'Email already exists for another employee' AS Message;
        RETURN;
    END

    UPDATE Employees
    SET 
        EmployeeName = @EmployeeName,
        Email = @Email,
        DepartmentId = @DepartmentId,
        Role = @Role
    WHERE EmployeeId = @EmployeeId;

    SELECT 1 AS Result, 'Employee updated successfully' AS Message;
END
GO

-- USP_DeleteEmployee
CREATE OR ALTER PROCEDURE USP_DeleteEmployee
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Employees 
    SET IsActive = 0 
    WHERE EmployeeId = @EmployeeId;

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT 0 AS Result, 'Employee not found' AS Message;
        RETURN;
    END

    SELECT 1 AS Result, 'Employee deactivated successfully' AS Message;
END
GO

-- USP_GetDepartments
CREATE OR ALTER PROCEDURE USP_GetDepartments
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        DepartmentId, 
        DepartmentName 
    FROM Departments 
    WHERE IsActive = 1 
    ORDER BY DepartmentName;
END
GO

-- USP_ValidateUser
CREATE OR ALTER PROCEDURE USP_ValidateUser
    @Email NVARCHAR(100),
    @Password NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        EmployeeId,
        EmployeeName,
        Email,
        Role,
        DepartmentId
    FROM Employees
    WHERE Email = @Email 
        AND Password = @Password 
        AND IsActive = 1;
END
GO

-- USP_ApplyLeave
CREATE OR ALTER PROCEDURE USP_ApplyLeave
    @EmployeeId INT,
    @LeaveType NVARCHAR(50),
    @FromDate DATE,
    @ToDate DATE,
    @Reason NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Result INT = 0;
    DECLARE @Message NVARCHAR(200) = '';
    
    IF EXISTS (
        SELECT 1 FROM LeaveRequests
        WHERE EmployeeId = @EmployeeId
            AND Status IN ('Pending', 'Approved')
            AND ((@FromDate BETWEEN FromDate AND ToDate)
                OR (@ToDate BETWEEN FromDate AND ToDate)
                OR (FromDate BETWEEN @FromDate AND @ToDate))
    )
    BEGIN
        SET @Result = 0;
        SET @Message = 'You already have a leave request for this date range';
    END
    ELSE IF @FromDate > @ToDate
    BEGIN
        SET @Result = 0;
        SET @Message = 'From date cannot be greater than To date';
    END
    ELSE
    BEGIN
        INSERT INTO LeaveRequests (EmployeeId, LeaveType, FromDate, ToDate, Reason, Status, AppliedDate)
        VALUES (@EmployeeId, @LeaveType, @FromDate, @ToDate, @Reason, 'Pending', GETDATE());
        
        SET @Result = 1;
        SET @Message = 'Leave request submitted successfully';
    END
    
    SELECT @Result AS Result, @Message AS Message;
END
GO

-- USP_ApproveRejectLeave
CREATE OR ALTER PROCEDURE USP_ApproveRejectLeave
    @LeaveId INT,
    @Status NVARCHAR(20),
    @ApprovedBy INT,
    @Remarks NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM LeaveRequests WHERE LeaveId = @LeaveId)
    BEGIN
        SELECT 0 AS Result, 'Leave request not found' AS Message;
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM LeaveRequests WHERE LeaveId = @LeaveId AND Status != 'Pending')
    BEGIN
        SELECT 0 AS Result, 'Leave request is already processed' AS Message;
        RETURN;
    END

    IF @Status NOT IN ('Approved', 'Rejected')
    BEGIN
        SELECT 0 AS Result, 'Invalid status. Must be Approved or Rejected' AS Message;
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM Employees WHERE EmployeeId = @ApprovedBy AND IsActive = 1)
    BEGIN
        SELECT 0 AS Result, 'Approver not found or inactive' AS Message;
        RETURN;
    END

    UPDATE LeaveRequests
    SET Status = @Status,
        ApprovedBy = @ApprovedBy,
        Remarks = @Remarks
    WHERE LeaveId = @LeaveId;

    SELECT 1 AS Result,
           CASE
               WHEN @Status = 'Approved' THEN 'Leave approved successfully'
               WHEN @Status = 'Rejected' THEN 'Leave rejected successfully'
               ELSE 'Leave processed successfully'
           END AS Message;
END
GO

-- USP_SearchLeaves
CREATE OR ALTER PROCEDURE USP_SearchLeaves
    @EmployeeNames NVARCHAR(MAX) = NULL,
    @Statuses NVARCHAR(MAX) = NULL,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @Role NVARCHAR(20),
    @EmployeeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        l.LeaveId,
        l.EmployeeId,
        e.EmployeeName,
        l.LeaveType,
        l.FromDate,
        l.ToDate,
        l.Reason,
        l.Status,
        l.AppliedDate,
        ISNULL(a.EmployeeName, '') AS ApprovedByName,
        ISNULL(l.Remarks, '') AS Remarks
    FROM LeaveRequests l
    INNER JOIN Employees e
        ON l.EmployeeId = e.EmployeeId
    LEFT JOIN Employees a
        ON l.ApprovedBy = a.EmployeeId
    WHERE
        (
    @Statuses IS NULL
    OR @Statuses = ''
    OR l.Status IN
    (
        SELECT value
        FROM STRING_SPLIT(@Statuses, ',')
    )
)
        AND
        (
            @Statuses IS NULL
            OR l.Status IN
            (
                SELECT value
                FROM STRING_SPLIT(@Statuses, ',')
            )
        )
        AND (@FromDate IS NULL OR l.FromDate >= @FromDate)
        AND (@ToDate IS NULL OR l.ToDate <= @ToDate)
        AND (@Role = 'Admin' OR l.EmployeeId = @EmployeeId)
    ORDER BY l.AppliedDate DESC;
END
GO

-- USP_GetLeaveDashboard
CREATE OR ALTER PROCEDURE USP_GetLeaveDashboard
    @Role NVARCHAR(20),
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @PendingCount INT = 0;
    DECLARE @ApprovedCount INT = 0;
    DECLARE @RejectedCount INT = 0;
    DECLARE @MonthlyCount INT = 0;
    
    IF @Role = 'Admin'
    BEGIN
        SELECT @PendingCount = COUNT(*) FROM LeaveRequests WHERE Status = 'Pending';
        SELECT @ApprovedCount = COUNT(*) FROM LeaveRequests WHERE Status = 'Approved';
        SELECT @RejectedCount = COUNT(*) FROM LeaveRequests WHERE Status = 'Rejected';
        SELECT @MonthlyCount = COUNT(*) FROM LeaveRequests 
        WHERE MONTH(AppliedDate) = MONTH(GETDATE()) AND YEAR(AppliedDate) = YEAR(GETDATE());
    END
    ELSE
    BEGIN
        SELECT @PendingCount = COUNT(*) FROM LeaveRequests WHERE EmployeeId = @EmployeeId AND Status = 'Pending';
        SELECT @ApprovedCount = COUNT(*) FROM LeaveRequests WHERE EmployeeId = @EmployeeId AND Status = 'Approved';
        SELECT @RejectedCount = COUNT(*) FROM LeaveRequests WHERE EmployeeId = @EmployeeId AND Status = 'Rejected';
        SELECT @MonthlyCount = COUNT(*) FROM LeaveRequests 
        WHERE EmployeeId = @EmployeeId 
            AND MONTH(AppliedDate) = MONTH(GETDATE()) 
            AND YEAR(AppliedDate) = YEAR(GETDATE());
    END
    
    SELECT @PendingCount AS PendingCount, 
           @ApprovedCount AS ApprovedCount, 
           @RejectedCount AS RejectedCount, 
           @MonthlyCount AS MonthlyCount;
END
GO

-- USP_GetLeaveById
CREATE OR ALTER PROCEDURE USP_GetLeaveById
    @LeaveId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        l.LeaveId,
        l.EmployeeId,
        e.EmployeeName,
        l.LeaveType,
        l.FromDate,
        l.ToDate,
        l.Reason,
        l.Status,
        l.AppliedDate,
        ISNULL(a.EmployeeName, '') AS ApprovedByName,
        ISNULL(l.Remarks, '') AS Remarks
    FROM LeaveRequests l
    INNER JOIN Employees e ON l.EmployeeId = e.EmployeeId
    LEFT JOIN Employees a ON l.ApprovedBy = a.EmployeeId
    WHERE l.LeaveId = @LeaveId;
END
GO

-- USP_GetLeaveSummary
CREATE OR ALTER PROCEDURE USP_GetLeaveSummary
    @EmployeeId INT
AS
BEGIN
    DECLARE @PendingLeaves INT = 0;
    DECLARE @ApprovedLeaves INT = 0;
    DECLARE @RejectedLeaves INT = 0;
    DECLARE @TotalDaysTaken INT = 0;
    
    SELECT @PendingLeaves = COUNT(*) FROM LeaveRequests 
    WHERE EmployeeId = @EmployeeId AND Status = 'Pending';
    
    SELECT @ApprovedLeaves = COUNT(*) FROM LeaveRequests 
    WHERE EmployeeId = @EmployeeId AND Status = 'Approved';
    
    SELECT @RejectedLeaves = COUNT(*) FROM LeaveRequests 
    WHERE EmployeeId = @EmployeeId AND Status = 'Rejected';
    
    SELECT @TotalDaysTaken = SUM(DATEDIFF(DAY, FromDate, ToDate) + 1) 
    FROM LeaveRequests 
    WHERE EmployeeId = @EmployeeId AND Status = 'Approved';
    
    SELECT @PendingLeaves AS PendingLeaves, @ApprovedLeaves AS ApprovedLeaves,
           @RejectedLeaves AS RejectedLeaves, ISNULL(@TotalDaysTaken, 0) AS TotalDaysTaken;
END
GO

-- USP_GetAdminCount
CREATE OR ALTER PROCEDURE USP_GetAdminCount
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) FROM Employees WHERE Role = 'Admin' AND IsActive = 1;
END
GO

-- ========================================
-- VERIFY DATA
-- ========================================

-- Check Employees
SELECT * FROM Employees;
GO

-- Check Leave Requests
SELECT * FROM LeaveRequests;
GO

-- Check Pending Leaves (Should show data)
SELECT COUNT(*) AS PendingLeaves FROM LeaveRequests WHERE Status = 'Pending';
GO

-- Check Dashboard Data
EXEC USP_GetLeaveDashboard 'Admin', 1;
GO