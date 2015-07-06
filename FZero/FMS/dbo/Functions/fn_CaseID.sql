



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fn_CaseID]
(@ID int)
RETURNS varchar(100)
AS
BEGIN
	

		Declare @Result varchar(500);
		set @Result='Case' + CONVERT(varchar, @ID);

		
		

	-- Return the result of the function
	RETURN @Result;

END