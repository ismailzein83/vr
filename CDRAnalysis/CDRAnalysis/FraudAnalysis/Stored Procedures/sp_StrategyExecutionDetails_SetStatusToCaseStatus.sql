
CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionDetails_SetStatusToCaseStatus]
	@AccountNumber VARCHAR(50),
	@CaseID INT,
	@OccuranceStatusID INT
AS
BEGIN
	UPDATE FraudAnalysis.StrategyExecutionDetails
	SET	CaseID = @CaseID,
		[SuspicionOccuranceStatus] = @OccuranceStatusID
	WHERE AccountNumber = @AccountNumber
		--AND CaseID IS NULL
END