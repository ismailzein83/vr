-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSourceLog_CreateTempForFiltered]
(
	@TempTableName VARCHAR(200),
	@DataSourceId INT = NULL,
	@Severities [integration].[SeverityType] READONLY,
	@From DATETIME = NULL,
	@To DATETIME = NULL
)
AS
BEGIN
	-- 'SET NOCOUNT ON' is added to prevent extra result sets from interfering with select statements
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			SELECT dsl.[ID],
			dsl.[DataSourceId],
			ds.[Name],
			dsl.[Severity],
			dsl.[Message],
			dsl.[LogEntryTime]
			INTO #RESULT
			FROM [FZeroLog].[integration].[DataSourceLog] dsl
			INNER JOIN [FZeroTransaction].[integration].[DataSource] ds ON dsl.DataSourceId = ds.ID
			WHERE 
				(@DataSourceId IS NULL OR dsl.DataSourceId = @DataSourceId) AND
				(dsl.Severity IN (SELECT Severity FROM @Severities)) AND
				(@From IS NULL OR dsl.LogEntryTime >= @From) AND
				(@To IS NULL OR dsl.LogEntryTime <= @To)
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END                    

	SET NOCOUNT OFF
END