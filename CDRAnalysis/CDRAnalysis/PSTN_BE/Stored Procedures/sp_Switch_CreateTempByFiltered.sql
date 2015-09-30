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
		
		SELECT s.ID,
			s.Name,
			s.TypeID,
			st.Name AS TypeName,
			s.AreaCode,
			s.TimeOffset,
			s.DataSourceID
		
		INTO #RESULT
		
		FROM PSTN_BE.Switch s
		INNER JOIN PSTN_BE.SwitchType st ON st.ID = s.TypeID
		
		WHERE (@Name IS NULL OR s.Name LIKE '%' + @Name + '%')
			AND (@SelectedTypeIDs IS NULL OR s.TypeID IN (SELECT TypeID FROM @TypeIDsTable))
			AND (@AreaCode IS NULL OR s.AreaCode LIKE '%' + @AreaCode + '%')
			
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
    END
    
    SET NOCOUNT OFF;
END