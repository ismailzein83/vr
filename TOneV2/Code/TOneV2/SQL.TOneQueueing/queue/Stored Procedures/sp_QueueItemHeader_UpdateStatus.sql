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
		LastUpdatedTime = GETDATE(),
		ProcessingTime = CASE WHEN @Status = 10 THEN GETDATE() ELSE ProcessingTime END,
		ProcessedTime = CASE WHEN @Status = 30 THEN GETDATE() ELSE ProcessedTime END
    WHERE
          ItemID = @ItemID
END