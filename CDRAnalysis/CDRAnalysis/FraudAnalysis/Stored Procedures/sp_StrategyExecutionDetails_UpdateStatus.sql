
CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionDetails_UpdateStatus]
	@AccountNumber VARCHAR(50),
	@CaseID INT,
	@OccuranceStatusID INT
AS
BEGIN
	UPDATE FraudAnalysis.StrategyExecutionDetails
	SET	CaseID = @CaseID,
		[SuspicionOccuranceStatus] = @OccuranceStatusID
	WHERE AccountNumber = @AccountNumber
		AND (CaseID IS NULL OR CaseID = @CaseID) -- true when an open or pending case is updated
END