

Create PROCEDURE [FraudAnalysis].[sp_StrategyExecutionItem_DeleteByStrategyExecutionID] 
    @StrategyExecutionID int
AS
BEGIN
        Delete from FraudAnalysis.StrategyExecutionItem  where StrategyExecutionID = @StrategyExecutionID
END

SET NOCOUNT OFF