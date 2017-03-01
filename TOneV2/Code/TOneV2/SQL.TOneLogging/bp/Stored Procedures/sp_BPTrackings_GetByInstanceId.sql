CREATE PROCEDURE [bp].[sp_BPTrackings_GetByInstanceId]	
	@ProcessInstanceID bigint,
	@LastTrackingId bigint,
	@Message varchar(max),
	@Severities varchar(max),
	@TOP int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @SeveritiesTable TABLE (Severity int)
	Insert @SeveritiesTable 
	SELECT  ParsedString FROM bp.ParseStringList(@Severities)
	
	
	IF @TOP is null
	BEGIN
	
		SELECT     [ID]
				  ,[ProcessInstanceID]
				  ,[ParentProcessID]
				  ,[TrackingMessage]
				  ,bpt.[Severity]
				  ,[EventTime]				  
				  ,[ExceptionDetail]
		FROM [bp].[BPTracking] as bpt WITH(NOLOCK) 
		LEFT JOIN @SeveritiesTable as sv on (sv.Severity = bpt.Severity )  
		where ( sv.Severity is not null OR (@Severities is null or @Severities = '') )
			AND bpt.ProcessInstanceID = @ProcessInstanceID 
			AND (@Message is NULL or  bpt.TrackingMessage LIKE '%' + @Message +'%' ) 
		ORDER BY ID DESC
	
	END
	ELSE
	BEGIN
		
		SELECT  TOP(@TOP) [ID]
				  ,[ProcessInstanceID]
				  ,[ParentProcessID]
				  ,[TrackingMessage]
				  ,bpt.[Severity]
				  ,[EventTime]
		FROM [bp].[BPTracking] as bpt WITH(NOLOCK) 
		LEFT JOIN @SeveritiesTable as sv on (sv.Severity = bpt.Severity )  
		where ( sv.Severity is not null OR (@Severities is null or @Severities = '') )
			AND bpt.ProcessInstanceID = @ProcessInstanceID 
			AND (@Message is NULL or  bpt.TrackingMessage LIKE '%' + @Message +'%' ) 
			AND (bpt.ID < @LastTrackingId or @LastTrackingId = 0)
		ORDER BY ID DESC
	END
END