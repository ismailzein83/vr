CREATE  FUNCTION [dbo].[DateOf](@datetime datetime)
RETURNS Datetime
AS
BEGIN
	

	return DATEADD(day,0,datediff(day,0, @datetime))
END