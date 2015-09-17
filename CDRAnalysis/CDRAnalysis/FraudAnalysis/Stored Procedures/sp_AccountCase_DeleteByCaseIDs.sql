

CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_DeleteByCaseIDs] 
    @CaseIDs varchar(1000)
AS
BEGIN

       IF (@CaseIDs IS NOT NULL)
			BEGIN
				DECLARE @CaseIDsTable TABLE (CaseID INT);
				INSERT INTO @CaseIDsTable (CaseID)
				SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@CaseIDs);
			END

    Delete ac from FraudAnalysis.AccountCase ac
	left join FraudAnalysis.StrategyExecutionDetails dt on ac.Id = dt.CaseId
	where dt.CaseId is null and  ac.ID in (select CaseID from @CaseIDsTable)
           
		
END

SET NOCOUNT OFF