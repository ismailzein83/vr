

CREATE PROCEDURE [FraudAnalysis].[bp_AccountStatus_GetNumbersByStatuses] 
@CaseStatusIDs varchar(100), 
@FromAccountNumber varchar(50),
@Top int
AS
BEGIN

DECLARE @CaseStatusIDsTable TABLE (CaseStatusID INT);

        IF (@CaseStatusIDs IS NOT NULL)
			BEGIN
				INSERT INTO @CaseStatusIDsTable (CaseStatusID)
				SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@CaseStatusIDs);
			END


SELECT TOP (@Top) accStatus.[AccountNumber]
  FROM [FraudAnalysis].[AccountStatus] accStatus
WHERE	(@CaseStatusIDs is null or accStatus.[Status] in (SELECT CaseStatusID FROM @CaseStatusIDsTable) ) 
and accStatus.AccountNumber >= @FromAccountNumber and (accStatus.validTill >= GETDATE() or accStatus.validTill is null) 
order by accStatus.[AccountNumber]
END