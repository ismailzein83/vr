﻿




CREATE PROCEDURE [FraudAnalysis].[sp_AccountStatus_GetByStatuses] 
@CaseStatusIDs varchar(100)
AS
BEGIN

DECLARE @CaseStatusIDsTable TABLE (CaseStatusID INT);

        IF (@CaseStatusIDs IS NOT NULL)
			BEGIN
				INSERT INTO @CaseStatusIDsTable (CaseStatusID)
				SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@CaseStatusIDs);
			END


SELECT accStatus.[AccountNumber]
      ,accStatus.[Status]
      ,accStatus.[AccountInfo]
  FROM [FraudAnalysis].[AccountStatus] accStatus
WHERE	(@CaseStatusIDs is null or accStatus.[Status] in (SELECT CaseStatusID FROM @CaseStatusIDsTable) ) 

END