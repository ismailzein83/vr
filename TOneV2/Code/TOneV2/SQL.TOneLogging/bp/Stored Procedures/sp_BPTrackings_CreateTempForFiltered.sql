CREATE PROCEDURE [bp].[sp_BPTrackings_CreateTempForFiltered]
	@TempTableName varchar(200),
	@ProcessInstanceId BIGINT,
	@Message varchar(max),
	@ArrSeverityID nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF OBJECT_ID(@TempTableName, N'U') IS NULL
	    BEGIN
		
			SELECT [ID]
				  ,[ProcessInstanceID]
				  ,[ParentProcessID]
				  ,[TrackingMessage]
				  ,[Severity]
				  ,[EventTime]					
				  ,[ExceptionDetail]
			INTO #RESULT
			FROM [bp].[BPTracking] WITH(NOLOCK)
			WHERE (@ArrSeverityID is NULL or Severity in (SELECT ParsedString FROM ParseStringList(@ArrSeverityID))) 
			And ProcessInstanceID = @ProcessInstanceId 
			AND (@Message is NULL or  TrackingMessage LIKE '%' + @Message +'%' ) 
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END

END