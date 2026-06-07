CREATE   PROCEDURE [dbo].[GetDepartments]
    @PageNumber INT,
    @PageSize   INT,
    @Search     NVARCHAR(255) = NULL,
    @Filter     NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    ;WITH DepartmentsCTE AS
    (
        SELECT
            d.ID AS Id,
            d.Code AS Code,
            d.Name AS Name,
            d.Description AS Description,
            d.IsDeleted AS IsDeleted,
            d.CreateDate AS CreatedDate,
            createdBy.FullName AS CreatedBy,
            d.LastModifiedDate AS LastModifiedDate,
            lastModifiedBy.FullName AS LastModifiedBy
        FROM Departments d
        LEFT JOIN Employees createdBy 
            ON createdBy.ID = d.CreatedBy
        LEFT JOIN Employees lastModifiedBy 
            ON lastModifiedBy.ID = d.LastModifiedBy
        WHERE
            d.IsDeleted = 0
            AND (
                @Search IS NULL
                OR d.Name LIKE '%' + @Search + '%'
                OR d.Code LIKE '%' + @Search + '%'
            )
            AND (
                @Filter IS NULL
                OR d.Code = @Filter   -- bạn đổi filter theo field mong muốn
            )
    )
    SELECT 
        d.*,
        @PageNumber AS PageNumber,
        @PageSize AS PageSize,
        COUNT(*) OVER() AS TotalRecords
    FROM DepartmentsCTE d
    ORDER BY d.CreatedDate DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
