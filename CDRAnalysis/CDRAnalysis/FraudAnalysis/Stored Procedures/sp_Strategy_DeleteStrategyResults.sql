
CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_DeleteStrategyResults] 
    @StrategyId int, 
	@FromDate DateTime,
	@ToDate DateTime
AS
BEGIN
		Delete from FraudAnalysis.[NumberProfile] where StrategyId = @StrategyId and FromDate>=@FromDate and ToDate <=@ToDate 
		Delete from FraudAnalysis.[AccountThreshold] where StrategyId = @StrategyId and DateDay>=@FromDate and DateDay <=@ToDate
END

SET NOCOUNT OFF