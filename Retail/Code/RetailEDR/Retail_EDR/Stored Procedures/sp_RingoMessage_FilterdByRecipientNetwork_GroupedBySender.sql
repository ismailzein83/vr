-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_EDR].[sp_RingoMessage_FilterdByRecipientNetwork_GroupedBySender]
	-- Add the parameters for the stored procedure here
	@From datetime,
	@To datetime
AS
BEGIN

	SELECT	Sender Name, count(*) 
	from	[Retail_EDR].[RingoMessage] 
	where	MessageDate >= @From and MessageDate < @To
			and RecipientNetwork ='ICSI' and Messagetype='1'
			group by (Sender)
END