-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSourceImportedBatch_Insert] 
	@DataSourceId uniqueidentifier,
	@BatchDescription nvarchar(1000),
	@BatchSize decimal(18, 5),
	@RecordsCount int,
	@MappingResult int,
	@MapperMessage nvarchar(max),
	@QueueItemIds nvarchar(50),
	@LogEntryTime dateTime
AS
BEGIN
DECLARE @NewItemID BIGINT

	Insert into integration.DataSourceImportedBatch(DataSourceId, BatchDescription, [BatchSize], RecordsCount, MappingResult, MapperMessage, QueueItemIds, LogEntryTime)
    values (@DataSourceId, @BatchDescription, @BatchSize, @RecordsCount, @MappingResult, @MapperMessage, @QueueItemIds, @LogEntryTime)
    
    SET @NewItemID = SCOPE_IDENTITY()
    select @NewItemID
END