


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fn_DatePeriod]
(@GivenDate datetime)
RETURNS varchar(500)
AS
BEGIN
	

		Declare @Result varchar(500);
		set @Result='Now'

		Declare @Week int;
		set @Week=DATEDIFF(second, @GivenDate, GETDATE()) / 60 / 60 / 24 / 7

		Declare @Day int;
		set @Day=DATEDIFF(second, @GivenDate, GETDATE()) / 60 / 60 / 24 % 7

		Declare @Hour int;
		set @Hour=DATEDIFF(second, @GivenDate, GETDATE()) / 60 / 60 % 24

		Declare @Minute int;
		set @Minute=DATEDIFF(second, @GivenDate, GETDATE()) / 60 % 60

		Declare @Second int;
		set @Second=DATEDIFF(second, @GivenDate, GETDATE()) % 60


		if ((@Week) >0)
		Begin

		-- Created 1 weeks, 2 days, 3 hours, 25 minutes and 20 seconds ago.
		set @Result= (SELECT  CAST(DATEDIFF(second, @GivenDate, GETDATE()) / 60 / 60 / 24 / 7 AS NVARCHAR(50)) + ' week(s) ago.');


		End


		if ((@Week) = 0 and (@Day) >0)
		Begin

		-- Created 1 weeks, 2 days, 3 hours, 25 minutes and 20 seconds ago.
		set @Result= (SELECT CAST(DATEDIFF(second, @GivenDate, GETDATE()) / 60 / 60 / 24 % 7 AS NVARCHAR(50)) + ' day(s) ago.');


		End


		if ((@Week) = 0 and (@Day) = 0 and (@Hour) >0)
		Begin

		-- Created 1 weeks, 2 days, 3 hours, 25 minutes and 20 seconds ago.
		set @Result= (SELECT CAST(DATEDIFF(second, @GivenDate, GETDATE()) / 60 / 60 % 24  AS NVARCHAR(50)) + ' hour(s) ago.');


		End


		if ((@Week) = 0 and (@Day) = 0 and (@Hour) = 0 and(@Minute) >0)
		Begin

		-- Created 1 weeks, 2 days, 3 hours, 25 minutes and 20 seconds ago.
		set @Result= (SELECT CAST(DATEDIFF(second, @GivenDate, GETDATE()) / 60 % 60 AS NVARCHAR(50)) + ' minute(s) ago.');


		End


		if ((@Week) = 0 and (@Day) = 0 and (@Hour) = 0 and(@Minute) = 0 and(@Second) >0)
		Begin

		-- Created 1 weeks, 2 days, 3 hours, 25 minutes and 20 seconds ago.
		set @Result= (SELECT CAST(DATEDIFF(second, @GivenDate, GETDATE()) % 60 AS NVARCHAR(50)) + ' second(s) ago.');


		End

		

	-- Return the result of the function
	RETURN @Result;

END