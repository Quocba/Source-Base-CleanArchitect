

CREATE   PROCEDURE [dbo].[GetRoles]
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
            r.ID AS Id,
            r.Name,
            COUNT(*) OVER() AS TotalRecords
        FROM [Role] r
        WHERE
           (@Search IS NULL OR r.Name LIKE '%' + @Search + '%')
    )
    SELECT * FROM RoleData
    ORDER BY Name ASC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
