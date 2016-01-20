

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_GetByCaseID]
	@CaseID int
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT 
		ID AS CaseID,
		AccountNumber,
		UserID,
		[Status] AS StatusID,
		StatusUpdatedTime,
		ValidTill,
		CreatedTime
		
	FROM FraudAnalysis.AccountCase
	
	WHERE ID = @CaseID
	
	ORDER BY CaseID DESC
END