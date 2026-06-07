CREATE   PROCEDURE [dbo].[GetPositions]
    @PageNumber INT,
    @PageSize   INT,
    @Search     NVARCHAR(255) = NULL,
    @Filter     NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    ;WITH PositionCTE AS (
        SELECT
            p.ID AS Id,
            p.Name AS Name,
            p.Description AS Description,
            p.IsDeleted AS IsDeleted
        FROM Positions p
        WHERE p.IsDeleted = 0
          AND (
                @Search IS NULL
                OR p.Name COLLATE Vietnamese_CI_AI LIKE N'%' + @Search + N'%'
              )
          AND ( @Filter IS NULL OR p.IsDeleted = CAST(@Filter AS BIT) )
    )
    SELECT
        p.*,
        @PageNumber AS PageNumber,
        @PageSize AS PageSize,
        COUNT(*) OVER() AS TotalRecords
    FROM PositionCTE p
    ORDER BY p.Id
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
