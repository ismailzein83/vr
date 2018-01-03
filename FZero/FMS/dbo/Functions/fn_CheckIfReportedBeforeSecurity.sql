



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fn_CheckIfReportedBeforeSecurity]
( @CLI varchar(50) )
RETURNS bit
AS
BEGIN
	
	-- Declare the return variable here
	DECLARE @Found bit;
	set @Found=0;
	
	
		
	
	
	IF EXISTS (
	
	SELECT     dbo.RecievedCalls.CLI, dbo.GeneratedCalls.ID
FROM         dbo.GeneratedCalls LEFT OUTER JOIN
                      dbo.RecievedCalls ON dbo.GeneratedCalls.ID = dbo.RecievedCalls.GeneratedCallID
WHERE     (dbo.GeneratedCalls.ReportingStatusSecurityID = 2) and RecievedCalls.CLI=@CLI

union 

select RelatedNumber as CLI, 0 as ID from RelatedNumbers where RelatedNumber=@CLI
	
	
	
	
	
	
	 )
	BEGIN
	set @Found=1;
	END

	-- Return the result of the function
	RETURN @Found;

END