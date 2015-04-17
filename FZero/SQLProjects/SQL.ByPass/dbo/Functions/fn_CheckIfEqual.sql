

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fn_CheckIfEqual]
(@CLI varchar(50), @b_number varchar(50))
RETURNS bit
AS
BEGIN
	
	-- Declare the return variable here
	DECLARE @IsEqual bit;
	set @IsEqual=0;
	
	
	IF (@CLI = @b_number)
	BEGIN
	set @IsEqual=1;
	END
	
		

	-- Return the result of the function
	RETURN @IsEqual;

END