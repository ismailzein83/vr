

Create PROCEDURE [FraudAnalysis].[sp_StrategyExecutionDetails_DeleteByStrategyExecutionID] 
    @StrategyExecutionID int
AS
BEGIN
        Delete from FraudAnalysis.StrategyExecutionDetails  where StrategyExecutionID = @StrategyExecutionID
END

SET NOCOUNT OFF