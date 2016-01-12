CREATE PROCEDURE [FraudAnalysis].[sp_DataWarehouse_GetAccountCases]
	@From datetime,
	@To datetime
AS
BEGIN
	SET NOCOUNT ON;
	
		SELECT        ac.ID AS CaseID, accSta.Status AS CaseStatus, FraudAnalysis.CallClass.NetType, stra.IsDefault, stra.PeriodID, exD.SuspicionLevelID, 
                         ac.CreatedTime AS CaseGenerationTime, stra.UserID AS StrategyUser, ac.UserID AS CaseUser, accSta.AccountNumber, stra.ID AS StrategyID
FROM            FraudAnalysis.StrategyExecution AS ex INNER JOIN
                         FraudAnalysis.Strategy AS stra ON ex.StrategyID = stra.ID INNER JOIN
                         FraudAnalysis.StrategyExecutionDetails AS exD INNER JOIN
                         FraudAnalysis.AccountCase AS ac ON exD.CaseID = ac.ID INNER JOIN
                         FraudAnalysis.AccountStatus AS accSta ON exD.AccountNumber = accSta.AccountNumber ON ex.ID = exD.StrategyExecutionID INNER JOIN
                         FraudAnalysis.CallClass ON ex.ID = FraudAnalysis.CallClass.ID
WHERE        (ac.CreatedTime BETWEEN @from AND @to)
END