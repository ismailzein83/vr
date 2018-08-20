-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_SubscriberPreview_Get]
	@ProcessInstanceId bigint
AS
BEGIN
	
	select SubscriberID, [Status],SubscriberProcessInstanceID,Details
	from TOneWhS_Sales.RP_Subscriber_Preview WITH(NOLOCK) 
	where ProcessInstanceID = @ProcessInstanceID
END