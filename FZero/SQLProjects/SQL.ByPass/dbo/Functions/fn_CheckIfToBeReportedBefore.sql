




-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fn_CheckIfToBeReportedBefore]
( @CLI varchar(50) )
RETURNS bit
AS
BEGIN
	
	-- Declare the return variable here
	DECLARE @Found bit;
	set @Found=0;
	
	
	IF EXISTS (SELECT     dbo.RecievedCalls.CLI, dbo.GeneratedCalls.ID
FROM         dbo.GeneratedCalls LEFT OUTER JOIN
                      dbo.RecievedCalls ON dbo.GeneratedCalls.ID = dbo.RecievedCalls.GeneratedCallID
WHERE     (dbo.GeneratedCalls.ReportingStatusID = 4) and dbo.RecievedCalls.CLI=@CLI )
	BEGIN
	set @Found=1;
	END

	-- Return the result of the function
	RETURN @Found;

END