﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSourceLog_CreateTempByFiltered]
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
			SELECT [ID],
			[DataSourceId],
			[Severity],
			[Message],
			[LogEntryTime]
			INTO #RESULT
			FROM [integration].[DataSourceLog] WITH(NOLOCK) 
			WHERE 
				(@DataSourceId IS NULL OR DataSourceId = @DataSourceId) AND
				(Severity IN (SELECT Severity FROM @Severities)) AND
				(@From IS NULL OR LogEntryTime >= @From) AND
				(@To IS NULL OR LogEntryTime <= @To)
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END                    

	SET NOCOUNT OFF
END