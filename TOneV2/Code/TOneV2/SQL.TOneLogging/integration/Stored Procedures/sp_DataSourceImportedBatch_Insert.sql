﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSourceImportedBatch_Insert] 
	@DataSourceId int,
	@BatchDescription nvarchar(1000),
	@BatchSize decimal(18, 5),
	@RecordsCount int,
	@MappingResult int,
	@MapperMessage nvarchar(1000),
	@QueueItemId nvarchar(50),
	@LogEntryTime dateTime
AS
BEGIN
DECLARE @NewItemID BIGINT

	Insert into integration.DataSourceImportedBatch(DataSourceId, BatchDescription, [BatchSize], RecordsCount, MappingResult, MapperMessage, QueueItemId, LogEntryTime)
    values (@DataSourceId, @BatchDescription, @BatchSize, @RecordsCount, @MappingResult, @MapperMessage, @QueueItemId, @LogEntryTime)
    
    SET @NewItemID = @@IDENTITY
    select @NewItemID
END