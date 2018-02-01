-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [TOneWhS_Sales].[sp_SubscriberPreview_Get]
	@ProcessInstanceId bigint
AS
BEGIN
	
	select SubscriberID, [Status], [Description]
	from TOneWhS_Sales.RP_Subscriber_Preview WITH(NOLOCK) 
	where ProcessInstanceID = @ProcessInstanceID
END