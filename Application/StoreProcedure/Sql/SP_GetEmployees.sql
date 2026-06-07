
CREATE     PROCEDURE [dbo].[GetEmployees]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @Search NVARCHAR(100) = NULL,
    @Status INT = NULL,
	@Filter NVARCHAR(255) = NULL
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
            e.Avatar,
            e.Bank,
            e.BankNo,
            e.BankAccountHolder,
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
        INNER JOIN Users u ON e.UserID = u.Id
        LEFT JOIN [Role] r ON u.RoleId = r.Id
        LEFT JOIN Departments d ON e.DepartmentId = d.Id
        LEFT JOIN Positions p ON e.PositionId = p.Id
        LEFT JOIN WareHouses w ON e.WareHouseId = w.Id
        LEFT JOIN Employees cb ON e.CreatedBy = cb.Id
        LEFT JOIN Employees lb ON e.LastModifiedBy = lb.Id
        WHERE (e.IsDeleted = 0 OR e.IsDeleted IS NULL)
          AND (
                @Search IS NULL 
                OR e.FullName LIKE '%' + @Search + '%'
                OR e.Code LIKE '%' + @Search + '%'
                OR e.Phone LIKE '%' + @Search + '%'
                OR e.Email LIKE '%' + @Search + '%'
              )
          AND (@Status IS NULL OR e.Status = @Status)
    )
    SELECT *
    FROM EmployeeData
    ORDER BY CreatedDate DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
