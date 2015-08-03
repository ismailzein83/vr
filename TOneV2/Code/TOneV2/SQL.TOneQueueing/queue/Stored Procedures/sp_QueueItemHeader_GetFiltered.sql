-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueItemHeader_GetFiltered]
	@QueueIDs VARCHAR(MAX),
	@Statuses VARCHAR(MAX),
	@DateFrom dateTime,
	@DateTo dateTime
AS
BEGIN

SELECT [ItemID]
      ,[QueueID]
      ,[ExecutionFlowTriggerItemID]
      ,[SourceItemID]
      ,[Description]
      ,[Status]
      ,[RetryCount]
      ,[ErrorMessage]
      ,[CreatedTime]
      ,[LastUpdatedTime]
  FROM [TOneQueueDB].[queue].[QueueItemHeader]
  WHERE (@QueueIDs is NULL or QueueID in (SELECT ParsedString FROM [queue].ParseStringList(@QueueIDs) ) ) and 
		(@Statuses is NULL or Status in (SELECT ParsedString FROM ParseStringList(@Statuses) ) ) and CreatedTime BETWEEN  @DateFrom and @DateTo
           
END