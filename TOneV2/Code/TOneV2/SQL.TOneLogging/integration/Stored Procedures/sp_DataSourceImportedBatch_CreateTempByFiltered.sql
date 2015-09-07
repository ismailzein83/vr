﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSourceImportedBatch_CreateTempByFiltered]
(
	@TempTableName VARCHAR(200),
	@DataSourceId INT = NULL,
	@BatchName NVARCHAR(1000) = NULL,
	@MappingResults [integration].[MappingResultType] READONLY,
	@From DATETIME = NULL,
	@To DATETIME = NULL
)
AS
BEGIN
	-- 'SET NOCOUNT ON' is added to prevent extra result sets from interfering with select statements
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			SELECT [ID],
			[BatchDescription],
			[BatchSize],
			[RecordsCount],
			[MappingResult],
			[MapperMessage],
			[QueueItemIds],
			[LogEntryTime]
			INTO #RESULT
			FROM [integration].[DataSourceImportedBatch]
			WHERE 
				(@DataSourceId IS NULL OR DataSourceId = @DataSourceId) AND
				(@BatchName IS NULL OR BatchDescription LIKE '%' + @BatchName + '%') AND
				MappingResult IN (SELECT MappingResult FROM @MappingResults) AND
				(@From IS NULL OR LogEntryTime >= @From) AND
				(@To IS NULL OR LogEntryTime <= @To)
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END                    

	SET NOCOUNT OFF
END