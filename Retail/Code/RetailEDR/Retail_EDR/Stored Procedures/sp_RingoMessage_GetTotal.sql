-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Retail_EDR.sp_RingoMessage_GetTotal
	-- Add the parameters for the stored procedure here
	@From datetime,
	@To datetime
AS
BEGIN

	--declare @Prev_From datetime = (select dateadd(mm, -1,@From))
	--declare @Prev_To datetime = (select dateadd(mm, -1,@To))

	SELECT	count(*) 
	from	[StandardRetail_EDR].[Retail_EDR].[RingoMessage] 
	where	MessageDate >= @From and MessageDate < @To
			and Sender ='ICSI' and Messagetype='1'

END