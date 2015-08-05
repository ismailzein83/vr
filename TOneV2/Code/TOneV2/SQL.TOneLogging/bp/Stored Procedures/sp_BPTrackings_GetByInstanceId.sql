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

	IF @TOP is null
	BEGIN
	
		SELECT     [ID]
				  ,[ProcessInstanceID]
				  ,[ParentProcessID]
				  ,[TrackingMessage]
				  ,[Severity]
				  ,[EventTime]
		FROM [bp].[BPTracking] as bpt WITH(NOLOCK)
		WHERE bpt.ProcessInstanceID = @ProcessInstanceID AND bpt.ID > @LastTrackingId 
			AND (@Message is NULL or  bpt.TrackingMessage LIKE '%' + @Message +'%' ) 
			AND (@Severities is NULL or Severity in (SELECT ParsedString FROM ParseStringList(@Severities) ) )
			ORDER BY ID DESC
	
	END
	ELSE
	BEGIN
		
		SELECT  TOP(@TOP) [ID]
				  ,[ProcessInstanceID]
				  ,[ParentProcessID]
				  ,[TrackingMessage]
				  ,[Severity]
				  ,[EventTime]
		FROM [bp].[BPTracking] as bpt WITH(NOLOCK)
		WHERE bpt.ProcessInstanceID = @ProcessInstanceID AND bpt.ID > @LastTrackingId 
			AND (@Message is NULL or  bpt.TrackingMessage LIKE '%' + @Message +'%' ) 
			AND (@Severities is NULL or Severity in (SELECT ParsedString FROM ParseStringList(@Severities) ) )
			ORDER BY ID DESC
	
	END
	

    
END