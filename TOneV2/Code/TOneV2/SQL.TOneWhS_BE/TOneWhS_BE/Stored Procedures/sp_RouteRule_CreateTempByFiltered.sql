CREATE PROCEDURE [TOneWhS_BE].[sp_RouteRule_CreateTempByFiltered]
	@TempTableName varchar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			SELECT
				  [ID]
				  ,[Criteria]
				  ,[TypeConfigID]
				  ,[RuleSettings]
				  ,[Description]
				  ,[BED]
				  ,[EED]
				  ,[ScheduleSettings]
			INTO #RESULT
			FROM TOneWhS_BE.RouteRule                           
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END
	SET NOCOUNT OFF
END