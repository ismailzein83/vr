-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_Switch_CreateTempByFiltered]
	@TempTableName VARCHAR(200) = NULL,
	@Name NVARCHAR(255) = NULL,
	@SelectedTypeIDs VARCHAR(MAX) = NULL,
	@AreaCode VARCHAR(10) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
    BEGIN
		IF @SelectedTypeIDs IS NOT NULL
		BEGIN
			DECLARE @TypeIDsTable TABLE (TypeID INT)
			INSERT INTO @TypeIDsTable (TypeID)
			SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@SelectedTypeIDs)
		END
		
		SELECT ID, Name, TypeID, AreaCode, TimeOffset
		
		INTO #RESULT FROM PSTN_BE.Switch
		
		WHERE (@Name IS NULL OR Name LIKE '%' + @Name + '%')
			AND (@SelectedTypeIDs IS NULL OR TypeID IN (SELECT TypeID FROM @TypeIDsTable))
			AND (@AreaCode IS NULL OR AreaCode LIKE '%' + @AreaCode + '%')
			
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
    END
    
    SET NOCOUNT OFF;
END