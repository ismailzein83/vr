-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueItemHeader_Update]
	@ItemID bigint,
	@Status int,
	@RetryCount int,
	@ErrorMessage nvarchar(max)
AS
BEGIN
	UPDATE [queue].[QueueItemHeader]
    SET   Status = @Status,
		ProcessingTime = CASE WHEN @Status = 10 THEN GETDATE() ELSE ProcessingTime END,
		ProcessedTime = CASE WHEN @Status = 30 THEN GETDATE() ELSE ProcessedTime END,
		  RetryCount = @RetryCount,
		  ErrorMessage = ISNULL(@ErrorMessage, ErrorMessage),
		  LastUpdatedTime = GETDATE()
    WHERE
          ItemID = @ItemID
END