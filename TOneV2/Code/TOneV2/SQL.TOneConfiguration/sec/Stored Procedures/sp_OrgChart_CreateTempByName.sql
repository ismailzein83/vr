-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_OrgChart_CreateTempByName]
	@TempTableName VARCHAR(200),
	@Name VARCHAR(100) = NULL
AS
BEGIN
	SET NOCOUNT ON;

    IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			SELECT ID, Name, Hierarchy
			INTO #RESULT
			FROM OrgChart
			WHERE
				(Name LIKE '%' + @Name + '%' OR @Name IS NULL)
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END

	SET NOCOUNT OFF
	
END