CREATE FUNCTION [dbo].[DateOf](@datetime datetime)
RETURNS Datetime
AS
BEGIN
	--declare @TheDate datetime
	--SET @TheDate = cast(
	--		(
	--			cast(Year(@datetime) AS varchar) 
	--			+ '-' + 
	--			cast(Month(@datetime)  AS varchar) 
	--			+ '-' +
	--			cast(Day(@datetime) AS varchar) 
	--		) AS datetime)

	--return @TheDate
	return DATEADD(day,0,datediff(day,0,@datetime))
END