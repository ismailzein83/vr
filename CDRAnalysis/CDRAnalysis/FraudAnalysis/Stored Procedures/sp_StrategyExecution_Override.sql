

CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecution_Override] 
    @StrategyId int, 
	@FromDate DateTime,
	@ToDate DateTime,
	@Id int out
AS
BEGIN
        SET @Id = (select top 1 ID from FraudAnalysis.StrategyExecution where StrategyId = @StrategyId and FromDate=@FromDate and ToDate =@ToDate order by ID  desc)
        
        update FraudAnalysis.StrategyExecution set IsOverriden=1 where Id = @Id 
END

SET NOCOUNT OFF