-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_SubscriberPreview_Insert]
	@ProcessInstanceId bigint,
	@SubscriberId int,
	@Status int,
    @SubscriberProcessInstanceId bigint
AS
BEGIN
	insert into TOneWhS_Sales.RP_Subscriber_Preview
	(
		ProcessInstanceID,
		SubscriberID,
		[Status],
		SubscriberProcessInstanceID
	)
	values (@ProcessInstanceId, @SubscriberId, @Status, @SubscriberProcessInstanceId)
END