-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchBrand_CreateTempByName]
	@TempTableName VARCHAR(200) = NULL,
	@Name VARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
    BEGIN
		SELECT ID, Name
		
		INTO #RESULT FROM PSTN_BE.SwitchBrand
		
		WHERE (@Name IS NULL OR Name LIKE '%' + @Name + '%')
			
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
    END
	
	SET NOCOUNT OFF;
END