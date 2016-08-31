CREATE PROCEDURE [bp].[sp_BPValidationMessage_CreateTempForFiltered]
	@TempTableName varchar(200),
	@ProcessInstanceId BIGINT,
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
				  ,[TargetKey]
				  ,[TargetType]
				  ,[Severity]
				  ,[Message]
			INTO #RESULT
			FROM bp.[BPValidationMessage] WITH(NOLOCK)
			WHERE (@ArrSeverityID is NULL or Severity in (SELECT ParsedString FROM ParseStringList(@ArrSeverityID))) 
			And ProcessInstanceID = @ProcessInstanceId 
			
			DECLARE @sql VARCHAR(4000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END

END