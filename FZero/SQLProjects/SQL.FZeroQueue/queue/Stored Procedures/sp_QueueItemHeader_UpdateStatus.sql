-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueItemHeader_UpdateStatus]
	@ItemID bigint,
	@Status int
AS
BEGIN
	UPDATE [queue].[QueueItemHeader]
    SET Status = @Status,
		LastUpdatedTime = GETDATE()
    WHERE
          ItemID = @ItemID
END