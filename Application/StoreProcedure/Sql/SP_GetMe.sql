
CREATE     PROCEDURE [dbo].[GetMe]
    @EmployeeId uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
     employee.ID AS Id,
	 employee.FullName as FullName,
	 employee.Email as Email,
	 employee.Gender as Gender,
	 employee.Phone as Phone,
	 employee.Address as Address,
	 employee.Avatar as Avatar,
	 employee.BankNo as BankNo,
	 employee.Bank as Bank,
	 employee.BankAccountHolder as BankAccountHolder,
	 employee.DBO as DBO
    FROM Employees employee
    WHERE employee.ID = @EmployeeId;
END
