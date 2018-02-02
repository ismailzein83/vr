Create PROCEDURE [bp].[sp_BPTrackings_GetBPInstanceTrackingMessages]	
	@ProcessInstanceID bigint,
	@Severities varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @SeveritiesTable TABLE (Severity int)
	Insert @SeveritiesTable 
	SELECT  ParsedString FROM bp.ParseStringList(@Severities)

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
		ORDER BY ID DESC
END