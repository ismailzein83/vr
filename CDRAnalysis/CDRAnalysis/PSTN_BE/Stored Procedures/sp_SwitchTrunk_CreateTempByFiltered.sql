-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchTrunk_CreateTempByFiltered] 
	@TempTableName VARCHAR(200) = NULL,
	@Name NVARCHAR(255) = NULL,
	@Symbol NVARCHAR(50) = NULL,
	@SelectedSwitchIDs VARCHAR(MAX) = NULL,
	@SelectedTypes VARCHAR(MAX) = NULL,
	@SelectedDirections VARCHAR(MAX) = NULL,
	@IsLinkedToTrunk BIT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
    BEGIN
		IF @SelectedSwitchIDs IS NOT NULL
		BEGIN
			DECLARE @SwitchIDsTable TABLE (SwitchID INT)
			INSERT INTO @SwitchIDsTable (SwitchID)
			SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@SelectedSwitchIDs)
		END
		
		IF @SelectedTypes IS NOT NULL
		BEGIN
			DECLARE @TypesTable TABLE ([Type] INT)
			INSERT INTO @TypesTable ([Type])
			SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@SelectedTypes)
		END
		
		IF @SelectedDirections IS NOT NULL
		BEGIN
			DECLARE @DirectionsTable TABLE (Direction INT)
			INSERT INTO @DirectionsTable (Direction)
			SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@SelectedDirections)
		END
		
		SELECT t.ID,
			t.Name,
			t.Symbol,
			t.SwitchID,
			s.Name AS SwitchName,
			t.[Type],
			t.Direction,
			t.LinkedToTrunkID,
			linkedTrunks.Name AS LinkedToTrunkName
		
		INTO #RESULT
		
		FROM PSTN_BE.SwitchTrunk t
		INNER JOIN PSTN_BE.Switch s ON s.ID = t.SwitchID
		LEFT JOIN PSTN_BE.SwitchTrunk linkedTrunks ON linkedTrunks.ID = t.LinkedToTrunkID
		
		WHERE (@Name IS NULL OR t.Name LIKE '%' + @Name + '%')
			AND (@Symbol IS NULL OR t.Symbol LIKE '%' + @Symbol + '%')
			AND (@SelectedSwitchIDs IS NULL OR t.SwitchID IN (SELECT SwitchID FROM @SwitchIDsTable))
			AND (@SelectedTypes IS NULL OR t.[Type] IN (SELECT [Type] FROM @TypesTable))
			AND (@SelectedDirections IS NULL OR t.Direction IN (SELECT Direction FROM @DirectionsTable))
			AND (@IsLinkedToTrunk IS NULL OR (@IsLinkedToTrunk = 0 AND t.LinkedToTrunkID IS NULL) OR (@IsLinkedToTrunk = 1 AND t.LinkedToTrunkID IS NOT NULL))
			
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
    END
	
	SET NOCOUNT OFF;
END