
CREATE PROCEDURE [queue].[sp_SummaryQueueItem_HasPendingItems]	
	@QueueIDs varchar(max),
	@From datetime,
	@To datetime,
	@HasPendingItems bit out
AS
BEGIN
	DECLARE @QueueIDsTable TABLE (QueueID int)
	INSERT INTO @QueueIDsTable (QueueID)
	select Convert(int, ParsedString) from [queue].[ParseStringList](@QueueIDs)

	IF NOT EXISTS(SELECT TOP 1 1 FROM [queue].[SummaryQueueItem] WITH(NOLOCK)
				  WHERE IsNULL(IsSuspended, 0) = 0 
				  AND (@QueueIDs  is null or QueueID in(select QueueID from @QueueIDsTable))
				  AND (BatchEnd > @From) AND (BatchStart < @To))
	BEGIN
		SET @HasPendingItems = 0
	END
	ELSE 
	BEGIN 
      SET @HasPendingItems = 1
	END 
END