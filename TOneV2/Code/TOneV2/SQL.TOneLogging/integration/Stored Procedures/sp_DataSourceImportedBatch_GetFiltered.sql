         -- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSourceImportedBatch_GetFiltered]
	
	@DataSourceId uniqueidentifier = NULL,
	@BatchName NVARCHAR(1000) = NULL,
	@MappingResults nvarchar(max),
	@From DATETIME = NULL,
	@To DATETIME = NULL,
	@Top int
AS

SET NOCOUNT ON;

	BEGIN
		DECLARE @MappingResultsTable TABLE (MappingResult int)
		INSERT INTO @MappingResultsTable (MappingResult)
		select Convert(int, ParsedString) from [bp].[ParseStringList](@MappingResults)

	select TOP(@Top) [ID]
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
	FROM [integration].[DataSourceImportedBatch] as ib WITH(NOLOCK) 
	WHERE 
		(@DataSourceId IS NULL OR DataSourceId = @DataSourceId) 
		AND (@BatchName IS NULL OR BatchDescription LIKE '%' + @BatchName + '%') 
		AND (@MappingResults is null  or ib.MappingResult in (select MappingResult from @MappingResultsTable))
		AND (@From IS NULL OR LogEntryTime >= @From) 
		AND (@To IS NULL OR LogEntryTime <= @To)
	ORDER BY ID DESC
	END
SET NOCOUNT OFF