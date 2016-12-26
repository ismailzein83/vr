-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Retail_EDR.sp_RingoMessage_FilterdBySenderNetwork_GroupedByRecipient
	-- Add the parameters for the stored procedure here
	@From datetime,
	@To datetime
AS
BEGIN

	SELECT	Recipient Name, count(*) as total
	from	[Retail_EDR].[RingoMessage] 
	where	MessageDate >= @From and MessageDate < @To
			and SenderNetwork ='ICSI' and StateRequest in ('1','8')
			group by (Recipient)
END