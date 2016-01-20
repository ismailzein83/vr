-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountInfo_GetByAccountStatuses]
	@CaseStatusIDs varchar(100) = NULL
AS
BEGIN

	DECLARE @CaseStatusIDsTable TABLE (CaseStatusID INT);

        IF (@CaseStatusIDs IS NOT NULL)
			BEGIN
				INSERT INTO @CaseStatusIDsTable (CaseStatusID)
				SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@CaseStatusIDs);
			END
			
	SELECT accInfo.[AccountNumber] 
	  ,accInfo.[InfoDetail]
	  
	FROM [FraudAnalysis].[AccountInfo] accInfo
	LEFT JOIN FraudAnalysis.AccountStatus accStatus ON accInfo.AccountNumber = accStatus.AccountNumber
	WHERE accStatus.AccountNumber IS NULL OR @CaseStatusIDs IS NULL OR accStatus.[Status] IN (SELECT CaseStatusID FROM @CaseStatusIDsTable)
END