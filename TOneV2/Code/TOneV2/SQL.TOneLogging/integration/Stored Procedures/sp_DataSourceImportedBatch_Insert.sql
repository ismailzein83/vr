-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSourceImportedBatch_Insert] 
	@DataSourceId uniqueidentifier,
	@BatchDescription nvarchar(1000),
	@BatchSize decimal(18, 5),
	@BatchState int,
	@IsDuplicateSameSize bit,
	@RecordsCount int,
	@MappingResult int,
	@MapperMessage nvarchar(max),
	@QueueItemIds nvarchar(50),
	@LogEntryTime dateTime,
	@BatchStart dateTime,
	@BatchEnd dateTime
AS
BEGIN
DECLARE @NewItemID BIGINT
  Insert into integration.DataSourceImportedBatch (DataSourceId, BatchDescription, [BatchSize], BatchState, IsDuplicateSameSize, RecordsCount, MappingResult, 
												   MapperMessage, QueueItemIds, LogEntryTime, BatchStart, BatchEnd)
  values (@DataSourceId, @BatchDescription, @BatchSize, @BatchState, @IsDuplicateSameSize, @RecordsCount, @MappingResult, @MapperMessage, @QueueItemIds, @LogEntryTime, @BatchStart, @BatchEnd)
   
  SET @NewItemID = SCOPE_IDENTITY()
  select @NewItemID
END