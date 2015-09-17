

CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecution_Override] 
    @StrategyId int, 
	@FromDate DateTime,
	@ToDate DateTime
AS
BEGIN
        update FraudAnalysis.StrategyExecution set IsOverriden=1 where Id = (select top 1 ID from FraudAnalysis.StrategyExecution with(nolock) where StrategyId = @StrategyId and FromDate=@FromDate and ToDate =@ToDate order by ID  desc)
END

SET NOCOUNT OFF