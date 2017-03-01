CREATE PROCEDURE [bp].[sp_BPTrackings_GetUpdated]
	@NbOfRows INT,
	@GreaterThanID BIGINT,
	@BPInstanceID INT,
	@Severities varchar(max)
AS
BEGIN
	DECLARE @SeveritiesTable TABLE (Severity int)
	Insert @SeveritiesTable 
	SELECT  ParsedString FROM bp.ParseStringList(@Severities)
	
IF (@GreaterThanID IS NULL)
	BEGIN
	SELECT TOP(@NbOfRows) [ID]
				  ,[ProcessInstanceID]
				  ,[ParentProcessID]
				  ,[TrackingMessage]
				  ,[Severity]
				  ,[EventTime]
				  ,[ExceptionDetail]
            INTO #temp_table
			FROM [bp].[BPTracking] WITH(NOLOCK) 
            WHERE ProcessInstanceID  = @BPInstanceID
            AND (@Severities is null or Severity in (select Severity from @SeveritiesTable))
            ORDER BY ID DESC
            
            SELECT * FROM #temp_table
	END
	
	ELSE
	BEGIN
	SELECT TOP(@NbOfRows) [ID]
				  ,[ProcessInstanceID]
				  ,[ParentProcessID]
				  ,[TrackingMessage]
				  ,[Severity]
				  ,[EventTime]
            INTO #temp2_table
            FROM [bp].[BPTracking] WITH(NOLOCK) 
			WHERE ProcessInstanceID  = @BPInstanceID 
			AND ID >@GreaterThanID
			AND (@Severities is null or Severity in (select Severity from @SeveritiesTable))
            ORDER BY ID
            
            SELECT * FROM #temp2_table
	END
END