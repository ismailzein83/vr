-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueItemHeader_UpdateStatuses]
	@ItemIDs varchar(max),
	@Status int
AS
BEGIN
	
	DECLARE @QueueItemIDsTable TABLE (ItemID bigint)
	INSERT INTO @QueueItemIDsTable (ItemID)
	select Convert(bigint, ParsedString) from [queue].[ParseStringList](@ItemIDs)
	
	UPDATE itemHeader
    SET Status = @Status,
		LastUpdatedTime = GETDATE()
    FROM [queue].[QueueItemHeader] itemHeader
    JOIN @QueueItemIDsTable ids ON itemHeader.ItemID = ids.ItemID
END