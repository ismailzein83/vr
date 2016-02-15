

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_BulkUpdate]
	@AccountCase [FraudAnalysis].AccountCaseType READONLY
AS
BEGIN

	
	UPDATE [FraudAnalysis].[AccountCase] 
	SET 
		AccountCase.UserID = ac.UserID,
		AccountCase.Status = ac.Status,
		AccountCase.StatusUpdatedTime = GETDATE()

	FROM [FraudAnalysis].[AccountCase]  inner join @AccountCase as ac ON  AccountCase.ID = ac.ID
	
	
END