
CREATE Procedure [integration].[sp_DataSourceImportedBatch_GetByDataSource]
	@DataSourceId uniqueidentifier,
	@From DateTime
AS
BEGIN
	SELECT [ID],[BatchDescription],[BatchSize],[BatchState],[RecordsCount],[MappingResult],[MapperMessage],[QueueItemIds],[LogEntryTime],[BatchStart],[BatchEnd]
	FROM [integration].[DataSourceImportedBatch]
	WHERE [DataSourceId] = @DataSourceId and [LogEntryTime] >= @From
END