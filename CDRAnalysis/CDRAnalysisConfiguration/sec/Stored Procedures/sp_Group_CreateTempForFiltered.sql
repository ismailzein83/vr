-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE PROCEDURE [sec].[sp_Group_CreateTempForFiltered]
	@TempTableName VARCHAR(200) = NULL,
	@Name VARCHAR(30) =  NULL
AS
BEGIN
	-- 'SET NOCOUNT ON' is added to prevent extra result sets from interfering with select statements
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			SELECT g.[ID],
			g.[Name],
			g.[Description]
			INTO #RESULT
			FROM [sec].[Group] g
			WHERE (@Name IS NULL OR g.Name LIKE '%' + @Name + '%')
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END
END