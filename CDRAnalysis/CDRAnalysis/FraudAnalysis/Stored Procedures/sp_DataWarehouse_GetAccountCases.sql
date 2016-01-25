CREATE PROCEDURE [FraudAnalysis].[sp_DataWarehouse_GetAccountCases]
	@From datetime,
	@To datetime
AS
BEGIN
	SET NOCOUNT ON;

	
		SELECT        ac.ID AS CaseID, FraudAnalysis.CallClass.NetType, stra.IsDefault, stra.PeriodID, exD.SuspicionLevelID, ac.CreatedTime AS CaseGenerationTime, 
								 stra.UserID AS StrategyUser, ac.UserID AS CaseUser, stra.ID AS StrategyID, ex.FromDate, ex.ToDate, ac.AccountNumber, ac.Status AS CaseStatus
		FROM            FraudAnalysis.StrategyExecution AS ex INNER JOIN
								 FraudAnalysis.Strategy AS stra ON ex.StrategyID = stra.ID INNER JOIN
								 FraudAnalysis.StrategyExecutionItem AS exD INNER JOIN
								 FraudAnalysis.AccountCase AS ac ON exD.CaseID = ac.ID ON ex.ID = exD.StrategyExecutionID INNER JOIN
								 FraudAnalysis.CallClass ON ex.ID = FraudAnalysis.CallClass.ID

		WHERE        (ex.FromDate >= @from AND ex.ToDate <=@to)
		order by ac.ID desc

END