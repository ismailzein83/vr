CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionDetails_DeleteByCaseIDs] 
    @CaseIDs varchar(1000)
AS
BEGIN

DECLARE @CaseIDsTable TABLE (CaseID INT);

        IF (@CaseIDs IS NOT NULL)
			BEGIN
				INSERT INTO @CaseIDsTable (CaseID)
				SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@CaseIDs);
			END

         Delete details from FraudAnalysis.StrategyExecutionDetails  details where (@CaseIDs is null or details.CaseID in (SELECT CaseID FROM @CaseIDsTable) ) 
        
END

SET NOCOUNT OFF