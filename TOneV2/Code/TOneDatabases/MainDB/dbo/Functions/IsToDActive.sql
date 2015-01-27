CREATE FUNCTION [dbo].[IsToDActive] 
(
	-- Add the parameters for the function here
	@HolidayDate smalldatetime,
	@WeekDay tinyint,
	@BeginTime varchar(12),
	@EndTime varchar(12),	
	@When datetime
)
RETURNS char(1)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result char(1)
	
	-- Get the time specified by @When in the format: hh:mm:ss.fff
	DECLARE @Time varchar(50) 
	SELECT @Time = Right(CONVERT(varchar(50), @when, 121), 12)

	SET @Result = 'Y'

	-- Check Holiday	
	IF @HolidayDate IS NOT NULL AND (Month(@When)<>Month(@HolidayDate) OR Day(@When)<>Day(@HolidayDate)) 
		SET @Result = 'N'

	-- Check Weekday (Weekday stored (from .NET is 0 based)
	IF @WeekDay IS NOT NULL AND ((datepart(weekday,@When)-1) <> @WeekDay) 
		SET @Result = 'N'

	-- Check Begin and End Times
	IF @BeginTime IS NOT NULL AND NOT (@Time BETWEEN @BeginTime AND @EndTime) 
		SET @Result = 'N'

	-- Return the result of the function
	RETURN @Result

END