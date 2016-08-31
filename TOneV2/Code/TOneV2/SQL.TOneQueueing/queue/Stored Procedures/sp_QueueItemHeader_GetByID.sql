-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueItemHeader_GetByID]
	@ItemID bigint,
	@QueueID int
AS
BEGIN
	SELECT [ItemID]
      ,[QueueID]
      ,ExecutionFlowTriggerItemID
      ,[SourceItemID]
      ,[Description]
      ,[Status]
      ,[RetryCount]
      ,[ErrorMessage]
      ,[CreatedTime]
      ,[LastUpdatedTime]
	FROM [queue].[QueueItemHeader] WITH(NOLOCK)
    WHERE
          ItemID = @ItemID AND QueueID = @QueueID
END