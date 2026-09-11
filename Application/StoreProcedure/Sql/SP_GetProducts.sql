CREATE OR ALTER PROCEDURE [dbo].[GetProducts]
    @PageNumber INT = 1,
    @PageSize   INT = 10,
    @Search     NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH ProductData AS (
        SELECT 
            p.Id,
            p.Code,
            p.Name,
            p.Description,
            p.Price,
            p.StockQuantity,
            p.Status,
            p.CategoryId,
            c.Name AS CategoryName,
            p.CreatedDate,
            p.LastModifiedDate,
            COUNT(*) OVER() AS TotalRecords
        FROM Products p
        LEFT JOIN Categories c ON p.CategoryId = c.Id
        WHERE p.IsDeleted = 0
          AND (
                @Search IS NULL 
                OR p.Name LIKE '%' + @Search + '%' 
                OR p.Code LIKE '%' + @Search + '%'
              )
    )
    SELECT * 
    FROM ProductData
    ORDER BY CreatedDate DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
