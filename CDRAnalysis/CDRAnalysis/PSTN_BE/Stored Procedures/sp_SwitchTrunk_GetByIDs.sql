-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_SwitchTrunk_GetByIDs]
	@TrunkIDs VARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @TrunkIDsTable TABLE (TrunkID INT)
	INSERT INTO @TrunkIDsTable (TrunkID)
	SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@TrunkIDs)
	
	SELECT ID, Name, SwitchID
	FROM PSTN_BE.SwitchTrunk
	WHERE ID IN (SELECT TrunkID FROM @TrunkIDsTable)
	
	SET NOCOUNT OFF;
END