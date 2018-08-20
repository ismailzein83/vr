-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_SubscriberPreview_Insert]
	@ProcessInstanceId bigint,
	@SubscriberId int,
	@Status int,
    @SubscriberProcessInstanceId bigint,
	@Details nvarchar(max)
AS
BEGIN
	insert into TOneWhS_Sales.RP_Subscriber_Preview
	(
		ProcessInstanceID,
		SubscriberID,
		[Status],
		SubscriberProcessInstanceID,
		Details
	)
	values (@ProcessInstanceId, @SubscriberId, @Status, @SubscriberProcessInstanceId,@Details)
END