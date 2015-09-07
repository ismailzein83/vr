-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_GetLastByAccountNumber]
	@AccountNumber VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT TOP 1
		ID AS CaseID,
		AccountNumber,
		UserID,
		[Status] AS StatusID,
		StatusUpdatedTime,
		ValidTill,
		CreatedTime
		
	FROM FraudAnalysis.AccountCase
	
	WHERE AccountNumber = @AccountNumber
	
	ORDER BY CaseID DESC
END