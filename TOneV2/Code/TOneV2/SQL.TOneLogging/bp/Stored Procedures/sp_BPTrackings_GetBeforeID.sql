CREATE PROCEDURE [bp].[sp_BPTrackings_GetBeforeID]
	@LessThanID BIGINT,
	@NbOfRows INT,
	@BPInstanceID INT,
	@Severities varchar(max)
AS
BEGIN	            
	DECLARE @SeveritiesTable TABLE (Severity int)
	Insert @SeveritiesTable 
	SELECT  ParsedString FROM bp.ParseStringList(@Severities)
	
	SELECT TOP(@NbOfRows) [ID]
				  ,[ProcessInstanceID]
				  ,[ParentProcessID]
				  ,[TrackingMessage]
				  ,[Severity]
				  ,[EventTime]				  
				  ,[ExceptionDetail]
	FROM [bp].[BPTracking] WITH(NOLOCK) 
	WHERE ProcessInstanceID = @BPInstanceID 
	AND ID < @LessThanID
	AND (@Severities is null or Severity in (select Severity from @SeveritiesTable))
	ORDER BY ID DESC
END