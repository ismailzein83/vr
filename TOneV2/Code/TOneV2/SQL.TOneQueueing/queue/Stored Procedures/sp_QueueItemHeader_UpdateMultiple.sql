-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueItemHeader_UpdateMultiple]
	@ItemIDs varchar(max),
	@Status int,
	@RetryCount int,
	@ErrorMessage nvarchar(max)
AS
BEGIN
	DECLARE @QueueItemIDsTable TABLE (ItemID bigint)
	INSERT INTO @QueueItemIDsTable (ItemID)
	select Convert(bigint, ParsedString) from [queue].[ParseStringList](@ItemIDs)

	UPDATE itemHeader
    SET   Status = @Status,
		ProcessingTime = CASE WHEN @Status = 10 THEN GETDATE() ELSE ProcessingTime END,
		ProcessedTime = CASE WHEN @Status = 30 THEN GETDATE() ELSE ProcessedTime END,
		  RetryCount = @RetryCount,
		  ErrorMessage = ISNULL(@ErrorMessage, ErrorMessage),
		  LastUpdatedTime = GETDATE()
    FROM [queue].[QueueItemHeader] itemHeader
    JOIN @QueueItemIDsTable ids ON itemHeader.ItemID = ids.ItemID
END