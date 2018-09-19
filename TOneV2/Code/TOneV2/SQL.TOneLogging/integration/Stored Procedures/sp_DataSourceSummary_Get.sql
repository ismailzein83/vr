CREATE PROCEDURE [integration].[sp_DataSourceSummary_Get]
	@DataSourceIDs varchar(max),
	@FromTime DateTime

AS
BEGIN

		DECLARE @EnabledDataSourcesTable TABLE (EnabledDataSourceID uniqueidentifier)
		INSERT INTO @EnabledDataSourcesTable (EnabledDataSourceID)
		select Convert(uniqueidentifier, ParsedString) from [integration].[ParseStringList](@DataSourceIDs)

	SELECT   	DSImportedBatch.[DataSourceId],
				MAX([LogEntryTime]) as LastImportedBatchTime,
				COUNT(*) as NbImportedBatch, 
				SUM([RecordsCount]) as TotalRecordCount,
				MAX([RecordsCount]) as MaxRecordCount,
				MIN([RecordsCount]) as MinRecordCount,
				MAX([BatchSize]) as MaxBatchSize,
				MIN([BatchSize]) as MinBatchSize,
				SUM(CASE WHEN [MappingResult] = 2 THEN 1 ELSE 0 END) as NbInvalidBatch,
				SUM(CASE WHEN [MappingResult] = 3 THEN 1 ELSE 0 END) as NbEmptyBatch
					
	FROM [TOneWFTracking].[integration].[DataSourceImportedBatch] DSImportedBatch

	WHERE [LogEntryTime] >= @FromTime
	 AND (DSImportedBatch.[DataSourceId] IN (select EnabledDataSourceID from @EnabledDataSourcesTable))

	GROUP BY [DataSourceId]

END