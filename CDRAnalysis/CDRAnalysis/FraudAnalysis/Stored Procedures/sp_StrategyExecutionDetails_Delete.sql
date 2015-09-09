

CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionDetails_Delete] 
    @StrategyExecutionID int
AS
BEGIN
        Delete from FraudAnalysis.StrategyExecutionDetails  where StrategyExecutionID = @StrategyExecutionID
END

SET NOCOUNT OFF