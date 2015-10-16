-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchTrunk_GetBySwitchIDs]
	@SwitchIDs VARCHAR(MAX) = NULL,
	@TrunkNameFilter NVARCHAR(255) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	IF @SwitchIDs IS NOT NULL
	BEGIN
		DECLARE @SwitchIDsTable TABLE (SwitchID INT)
		INSERT INTO @SwitchIDsTable (SwitchID)
		SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@SwitchIDs)
	END
	
	SELECT ID, Name, SwitchID
	FROM PSTN_BE.SwitchTrunk
	WHERE (@SwitchIDs IS NULL OR SwitchID IN (SELECT SwitchID FROM @SwitchIDsTable))
		AND (@TrunkNameFilter IS NULL OR Name LIKE '%' + @TrunkNameFilter + '%')
	
	SET NOCOUNT OFF;
END