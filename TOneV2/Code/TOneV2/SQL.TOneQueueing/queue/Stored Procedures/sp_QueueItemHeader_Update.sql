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
		  RetryCount = @RetryCount,
		  ErrorMessage = ISNULL(@ErrorMessage, ErrorMessage),
		  LastUpdatedTime = GETDATE()
    WHERE
          ItemID = @ItemID
END