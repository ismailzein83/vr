         -- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSourceImportedBatch_GetAfterID]
	@Top int,
	@AfterId bigint = NULL,
	@ExecutionsStatus VARCHAR(MAX) = NULL
AS

BEGIN
	DECLARE @ExecutionsStatusTable TABLE (ExecutionStatus int)
	Insert @ExecutionsStatusTable 
	SELECT  ParsedString FROM [integration].[ParseStringList](@ExecutionsStatus)

	select TOP (@Top) [ID]
	,[BatchDescription]
	,[BatchSize]
	,[BatchState]
	,[RecordsCount]
	,[MappingResult]
	,[MapperMessage]
	,[QueueItemIds]
	,[LogEntryTime]
	,[BatchStart]
	,[BatchEnd]
	,[ExecutionStatus]
	FROM [integration].[DataSourceImportedBatch] WITH(NOLOCK) 
	WHERE (@AfterId is NULL OR ID > @AfterId)
	  AND (@ExecutionsStatus IS NULL OR ExecutionStatus IS NULL OR ExecutionStatus IN (Select ExecutionStatus FROM @ExecutionsStatusTable) )
	  ORDER BY ID
END