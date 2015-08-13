-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_CreateTempForFiltered]
	@TempTableName VARCHAR(200)
AS
BEGIN
	SET NOCOUNT ON;

    IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			SELECT ds.[ID],
			ds.[Name],
			ds.[adapterID],
			ad.[Info],
			ds.[TaskId],
			ds.[Settings]
			
			INTO #RESULT
			
			FROM [integration].[DataSource] AS ds
			JOIN adapterType AS ad ON ds.adapterID = ad.ID
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END

	SET NOCOUNT OFF
END