CREATE PROCEDURE [bp].[sp_BPTrackings_CreateTempForFiltered]
	@TempTableName varchar(200),	
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

	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
		
			SELECT TOP(@TOP) [ID]
				  ,[ProcessInstanceID]
				  ,[ParentProcessID]
				  ,[TrackingMessage]
				  ,[Severity]
				  ,[EventTime]
			INTO #RESULT
			FROM [bp].[BPTracking] as bpt WITH(NOLOCK)
			WHERE bpt.ProcessInstanceID = @ProcessInstanceID AND bpt.ID > @LastTrackingId 
			AND (@Message is NULL or  bpt.TrackingMessage LIKE '%' + @Message +'%' ) 
			AND (@Severities is NULL or Severity in (SELECT ParsedString FROM ParseStringList(@Severities) ) )
			ORDER BY ID DESC
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END	
	
END