-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_Update]
	@CaseID INT,
	@UserID INT,
	@CaseStatusID INT,
	@ValidTill DATETIME
AS
BEGIN
	UPDATE FraudAnalysis.AccountCase
	SET UserID = @UserID,
		[Status] = @CaseStatusID,
		StatusUpdatedTime = GETDATE(),
		ValidTill = @ValidTill
	WHERE ID = @CaseID
END