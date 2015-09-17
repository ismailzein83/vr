
CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_GetNamesByIDs] 
	@StrategyIDs VARCHAR(MAX) = NULL
AS
BEGIN
	IF (@StrategyIDs IS NOT NULL)
	BEGIN
		DECLARE @StrategyIDsTable TABLE (StrategyID INT);
		INSERT INTO @StrategyIDsTable (StrategyID)
		SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@StrategyIDs);
	END
 
	SELECT [Name] FROM FraudAnalysis.Strategy
	WHERE (@StrategyIDs IS NULL OR ID IN (SELECT StrategyID FROM @StrategyIDsTable))
END