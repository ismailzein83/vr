
CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_Cancel]
	@StrategyID INT,
	@AccountNumber varchar(50),
	@From DATETIME,
	@To DATETIME
AS
BEGIN
	Update  ac
		set ac.Status = 5
		from FraudAnalysis.AccountCase ac
		inner join FraudAnalysis.StrategyExecutionDetails sed on sed.CaseId=ac.ID
		inner join FraudAnalysis.StrategyExecution se on se.ID=sed.StrategyExecutionID
		where  (@AccountNumber is null or ac.AccountNumber=@AccountNumber) 
		and    ac.CreatedTime between @From and @To
		and    (@StrategyID =0  or se.StrategyID=@StrategyID)
END