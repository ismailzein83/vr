



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fn_CheckIfLocal]
(@CLI varchar(50))
RETURNS bit
AS
BEGIN
	
	-- Declare the return variable here
	DECLARE @IsLocal bit;
	set @IsLocal=0;
	
	
	IF (@CLI like '+%' or @CLI like '00%' or @CLI like '964%' or @CLI like '963%' or @CLI like '240%' or @CLI ='')
	BEGIN
	set @IsLocal=0;
	END
	
	ELSE IF( (LEN (@CLI)=11 and   @CLI like '07%') or (LEN (@CLI)=10 and   @CLI like '09%')  or (LEN (@CLI)=10 and   @CLI like '055%')  or (LEN (@CLI)=10 and   @CLI like '022%'))
	BEGIN
	set @IsLocal=1;
	END
	

	-- Return the result of the function
	RETURN @IsLocal;

END