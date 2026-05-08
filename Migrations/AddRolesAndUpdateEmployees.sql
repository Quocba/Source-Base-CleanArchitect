-- =============================================
-- Pre-Migration Script: Thêm Table Role và RoleId cho Users
-- Date: 2026-02-02
-- QUAN TRỌNG: Chạy script này TRƯỚC KHI chạy EF Migration
-- =============================================

-- 1. Tạo bảng Role (nếu chưa tồn tại)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Role')
BEGIN
    CREATE TABLE [dbo].[Role] (
        [ID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(255) NOT NULL
    );
    PRINT 'Created table Role';
END
GO

-- 2. Insert 2 Role: Admin và Manager (cùng ID với data seeding trong code)
IF NOT EXISTS (SELECT * FROM [dbo].[Role] WHERE [ID] = '11111111-1111-1111-1111-111111111111')
BEGIN
    INSERT INTO [dbo].[Role] ([ID], [Name]) VALUES ('11111111-1111-1111-1111-111111111111', N'Admin');
    PRINT 'Inserted Admin role';
END

IF NOT EXISTS (SELECT * FROM [dbo].[Role] WHERE [ID] = '22222222-2222-2222-2222-222222222222')
BEGIN
    INSERT INTO [dbo].[Role] ([ID], [Name]) VALUES ('22222222-2222-2222-2222-222222222222', N'Manager');
    PRINT 'Inserted Manager role';
END
GO

-- 3. Thêm cột RoleId vào bảng Users (nullable trước)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'RoleId')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [RoleId] UNIQUEIDENTIFIER NULL;
    PRINT 'Added RoleId column to Users table';
END
GO

-- 4. Cập nhật tất cả Users hiện tại với Role MANAGER làm mặc định
UPDATE [dbo].[Users] 
SET [RoleId] = '22222222-2222-2222-2222-222222222222' 
WHERE [RoleId] IS NULL;
PRINT 'Updated existing users with Manager role';
GO

-- 5. Thêm NOT NULL constraint cho RoleId
ALTER TABLE [dbo].[Users] ALTER COLUMN [RoleId] UNIQUEIDENTIFIER NOT NULL;
PRINT 'Set RoleId as NOT NULL';
GO

-- 6. Thêm Foreign Key constraint (nếu chưa có)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Users_Role_RoleId')
BEGIN
    ALTER TABLE [dbo].[Users] 
    ADD CONSTRAINT [FK_Users_Role_RoleId] 
    FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role]([ID]) 
    ON DELETE NO ACTION;
    PRINT 'Added FK constraint FK_Users_Role_RoleId';
END
GO

-- 7. Thêm các cột Bank info vào bảng Employees
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'Bank')
BEGIN
    ALTER TABLE [dbo].[Employees] ADD [Bank] NVARCHAR(255) NULL;
    PRINT 'Added Bank column to Employees table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'BankNo')
BEGIN
    ALTER TABLE [dbo].[Employees] ADD [BankNo] NVARCHAR(255) NULL;
    PRINT 'Added BankNo column to Employees table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name = 'BankAccountHolder')
BEGIN
    ALTER TABLE [dbo].[Employees] ADD [BankAccountHolder] NVARCHAR(255) NULL;
    PRINT 'Added BankAccountHolder column to Employees table';
END
GO

PRINT '========================================';
PRINT 'Pre-migration script completed!';
PRINT 'Updating GetEmployees stored procedure...';
PRINT '========================================';
GO

-- 8. Cập nhật stored procedure GetEmployees để trả về Bank info
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'GetEmployees')
BEGIN
    DROP PROCEDURE [dbo].[GetEmployees];
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetEmployees]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @Search NVARCHAR(100) = NULL,
    @Filter NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH EmployeeData AS (
        SELECT 
            e.Id,
            e.Code,
            e.FullName,
            e.Gender,
            e.Phone,
            e.Email,
            e.Address,
            u.RoleId,
            r.Name AS Role,
            e.MaritalStatus,
            e.Nationality,
            e.Avatar,
            e.Bank,
            e.BankNo,
            e.BankAccountHolder,
            e.HireDate,
            e.Status,
            e.DepartmentId,
            d.Name AS Department,
            e.PositionId,
            p.Name AS Position,
            e.WareHouseId,
            w.Name AS WareHouse,
            w.Address AS WareHouseAddress,
            w.Area,
            e.IsDeleted,
            e.CreatedDate,
            e.LastModifiedDate,
            cb.FullName AS CreatedBy,
            lb.FullName AS LastModifiedBy,
            COUNT(*) OVER() AS TotalRecords
        FROM Employees e
        INNER JOIN Users u ON e.Id = u.Id
        LEFT JOIN Role r ON u.RoleId = r.Id
        LEFT JOIN Departments d ON e.DepartmentId = d.Id
        LEFT JOIN Positions p ON e.PositionId = p.Id
        LEFT JOIN WareHouses w ON e.WareHouseId = w.Id
        LEFT JOIN Employees cb ON e.CreatedBy = cb.Id
        LEFT JOIN Employees lb ON e.LastModifiedBy = lb.Id
        WHERE (e.IsDeleted = 0 OR e.IsDeleted IS NULL)
            AND (@Search IS NULL OR e.FullName LIKE '%' + @Search + '%' 
                 OR e.Code LIKE '%' + @Search + '%'
                 OR e.Phone LIKE '%' + @Search + '%'
                 OR e.Email LIKE '%' + @Search + '%')
            AND (@Filter IS NULL OR e.Status = @Filter)
    )
    SELECT * FROM EmployeeData
    ORDER BY CreatedDate DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

PRINT 'GetEmployees stored procedure updated successfully!';

PRINT '========================================';
PRINT 'Creating GetRoles stored procedure...';
PRINT '========================================';
GO

-- 9. Tạo stored procedure GetRoles (cho GetRoleQueryHandle)
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'GetRoles')
BEGIN
    DROP PROCEDURE [dbo].[GetRoles];
END
GO

CREATE PROCEDURE [dbo].[GetRoles]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @Search NVARCHAR(100) = NULL,
    @Filter NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH RoleData AS (
        SELECT 
            r.ID AS Id, -- Map ID -> Id
            r.Name,
            COUNT(*) OVER() AS TotalRecords
        FROM Role r
        WHERE (@Search IS NULL OR r.Name LIKE '%' + @Search + '%')
    )
    SELECT * FROM RoleData
    ORDER BY Name ASC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

PRINT 'GetRoles stored procedure created successfully!';

PRINT '========================================';
PRINT 'Updating GetEmployees and GetEmployeesByWareHouse (Remove Old Fields, Add DBO)...';
PRINT '========================================';
GO

-- 10. Update GetEmployees (Remove MaritalStatus/Nationality, Add DBO)
CREATE OR ALTER PROCEDURE [dbo].[GetEmployees]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @Search NVARCHAR(100) = NULL,
    @Filter NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH EmployeeData AS (
        SELECT 
            e.Id,
            e.Code,
            e.FullName,
            e.Gender,
            e.Phone,
            e.Email,
            e.Address,
            u.RoleId,
            r.Name AS Role,
            -- Fields Updated here
            e.DBO,
            e.Avatar,
            e.Bank,
            e.BankNo,
            e.BankAccountHolder,
            e.HireDate,
            e.Status,
            e.DepartmentId,
            d.Name AS Department,
            e.PositionId,
            p.Name AS Position,
            e.WareHouseId,
            w.Name AS WareHouse,
            w.Address AS WareHouseAddress,
            w.Area,
            e.IsDeleted,
            e.CreatedDate,
            e.LastModifiedDate,
            cb.FullName AS CreatedBy,
            lb.FullName AS LastModifiedBy,
            COUNT(*) OVER() AS TotalRecords
        FROM Employees e
        INNER JOIN Users u ON e.Id = u.Id
        LEFT JOIN Role r ON u.RoleId = r.Id
        LEFT JOIN Departments d ON e.DepartmentId = d.Id
        LEFT JOIN Positions p ON e.PositionId = p.Id
        LEFT JOIN WareHouses w ON e.WareHouseId = w.Id
        LEFT JOIN Employees cb ON e.CreatedBy = cb.Id
        LEFT JOIN Employees lb ON e.LastModifiedBy = lb.Id
        WHERE (e.IsDeleted = 0 OR e.IsDeleted IS NULL)
            AND (@Search IS NULL OR e.FullName LIKE '%' + @Search + '%' 
                 OR e.Code LIKE '%' + @Search + '%'
                 OR e.Phone LIKE '%' + @Search + '%'
                 OR e.Email LIKE '%' + @Search + '%')
            AND (@Filter IS NULL OR e.Status = @Filter)
    )
    SELECT * FROM EmployeeData
    ORDER BY CreatedDate DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

PRINT 'GetEmployees updated successfully!';

-- 11. Update/Create GetEmployeesByWareHouse
CREATE OR ALTER PROCEDURE [dbo].[GetEmployeesByWareHouse]
    @WareHouseId UNIQUEIDENTIFIER,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @Search NVARCHAR(100) = NULL,
    @Filter NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH EmployeeData AS (
        SELECT 
            e.Id,
            e.Code,
            e.FullName,
            e.Gender,
            e.Phone,
            e.Email,
            e.Address,
            u.RoleId,
            r.Name AS Role,
            e.DBO,
            e.Avatar,
            e.Bank,
            e.BankNo,
            e.BankAccountHolder,
            e.HireDate,
            e.Status,
            e.DepartmentId,
            d.Name AS Department,
            e.PositionId,
            p.Name AS Position,
            e.WareHouseId,
            w.Name AS WareHouse,
            w.Address AS WareHouseAddress,
            w.Area,
            e.IsDeleted,
            e.CreatedDate,
            e.LastModifiedDate,
            cb.FullName AS CreatedBy,
            lb.FullName AS LastModifiedBy,
            COUNT(*) OVER() AS TotalRecords
        FROM Employees e
        INNER JOIN Users u ON e.Id = u.Id
        LEFT JOIN Role r ON u.RoleId = r.Id
        LEFT JOIN Departments d ON e.DepartmentId = d.Id
        LEFT JOIN Positions p ON e.PositionId = p.Id
        LEFT JOIN WareHouses w ON e.WareHouseId = w.Id
        LEFT JOIN Employees cb ON e.CreatedBy = cb.Id
        LEFT JOIN Employees lb ON e.LastModifiedBy = lb.Id
        WHERE (e.IsDeleted = 0 OR e.IsDeleted IS NULL)
            AND e.WareHouseId = @WareHouseId
            AND (@Search IS NULL OR e.FullName LIKE '%' + @Search + '%' 
                 OR e.Code LIKE '%' + @Search + '%'
                 OR e.Phone LIKE '%' + @Search + '%'
                 OR e.Email LIKE '%' + @Search + '%')
            AND (@Filter IS NULL OR e.Status = @Filter)
    )
    SELECT * FROM EmployeeData
    ORDER BY CreatedDate DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

PRINT 'GetEmployeesByWareHouse updated successfully!';

PRINT '========================================';
PRINT 'Updating GetEmployees and GetEmployeesByWareHouse (Remove HireDate)...';
PRINT '========================================';
GO

-- 12. Update GetEmployees (Remove HireDate)
CREATE OR ALTER PROCEDURE [dbo].[GetEmployees]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @Search NVARCHAR(100) = NULL,
    @Filter NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH EmployeeData AS (
        SELECT 
            e.Id,
            e.Code,
            e.FullName,
            e.Gender,
            e.Phone,
            e.Email,
            e.Address,
            u.RoleId,
            r.Name AS Role,
            e.DBO,
            e.Avatar,
            e.Bank,
            e.BankNo,
            e.BankAccountHolder,
            -- Removed e.HireDate
            e.Status,
            e.DepartmentId,
            d.Name AS Department,
            e.PositionId,
            p.Name AS Position,
            e.WareHouseId,
            w.Name AS WareHouse,
            w.Address AS WareHouseAddress,
            w.Area,
            e.IsDeleted,
            e.CreatedDate,
            e.LastModifiedDate,
            cb.FullName AS CreatedBy,
            lb.FullName AS LastModifiedBy,
            COUNT(*) OVER() AS TotalRecords
        FROM Employees e
        INNER JOIN Users u ON e.UserId = u.Id
        LEFT JOIN Role r ON u.RoleId = r.Id
        LEFT JOIN Departments d ON e.DepartmentId = d.Id
        LEFT JOIN Positions p ON e.PositionId = p.Id
        LEFT JOIN WareHouses w ON e.WareHouseId = w.Id
        LEFT JOIN Employees cb ON e.CreatedBy = cb.Id
        LEFT JOIN Employees lb ON e.LastModifiedBy = lb.Id
        WHERE (e.IsDeleted = 0 OR e.IsDeleted IS NULL)
            AND (@Search IS NULL OR e.FullName LIKE '%' + @Search + '%' 
                 OR e.Code LIKE '%' + @Search + '%'
                 OR e.Phone LIKE '%' + @Search + '%'
                 OR e.Email LIKE '%' + @Search + '%')
            AND (@Filter IS NULL OR e.Status = @Filter)
    )
    SELECT * FROM EmployeeData
    ORDER BY CreatedDate DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

PRINT 'GetEmployees updated successfully!';

-- 13. Update GetEmployeesByWareHouse (Remove HireDate)
CREATE OR ALTER PROCEDURE [dbo].[GetEmployeesByWareHouse]
    @WareHouseId UNIQUEIDENTIFIER,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @Search NVARCHAR(100) = NULL,
    @Filter NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH EmployeeData AS (
        SELECT 
            e.Id,
            e.Code,
            e.FullName,
            e.Gender,
            e.Phone,
            e.Email,
            e.Address,
            u.RoleId,
            r.Name AS Role,
            e.DBO,
            e.Avatar,
            e.Bank,
            e.BankNo,
            e.BankAccountHolder,
            -- Removed e.HireDate
            e.Status,
            e.DepartmentId,
            d.Name AS Department,
            e.PositionId,
            p.Name AS Position,
            e.WareHouseId,
            w.Name AS WareHouse,
            w.Address AS WareHouseAddress,
            w.Area,
            e.IsDeleted,
            e.CreatedDate,
            e.LastModifiedDate,
            cb.FullName AS CreatedBy,
            lb.FullName AS LastModifiedBy,
            COUNT(*) OVER() AS TotalRecords
        FROM Employees e
        INNER JOIN Users u ON e.UserId = u.Id
        LEFT JOIN Role r ON u.RoleId = r.Id
        LEFT JOIN Departments d ON e.DepartmentId = d.Id
        LEFT JOIN Positions p ON e.PositionId = p.Id
        LEFT JOIN WareHouses w ON e.WareHouseId = w.Id
        LEFT JOIN Employees cb ON e.CreatedBy = cb.Id
        LEFT JOIN Employees lb ON e.LastModifiedBy = lb.Id
        WHERE (e.IsDeleted = 0 OR e.IsDeleted IS NULL)
            AND e.WareHouseId = @WareHouseId
            AND (@Search IS NULL OR e.FullName LIKE '%' + @Search + '%' 
                 OR e.Code LIKE '%' + @Search + '%'
                 OR e.Phone LIKE '%' + @Search + '%'
                 OR e.Email LIKE '%' + @Search + '%')
            AND (@Filter IS NULL OR e.Status = @Filter)
    )
    SELECT * FROM EmployeeData
    ORDER BY CreatedDate DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

PRINT 'GetEmployeesByWareHouse updated successfully!';
