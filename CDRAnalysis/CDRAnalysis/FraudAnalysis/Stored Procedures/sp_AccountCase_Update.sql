-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_Update]
	@CaseID INT,
	@UserID INT,
	@CaseStatusID INT,
	@ValidTill DATETIME,
	@Reason VARCHAR(MAX) = NULL
AS
BEGIN
	UPDATE FraudAnalysis.AccountCase
	SET UserID = @UserID,
		[Status] = @CaseStatusID,
		StatusUpdatedTime = GETDATE(),
		ValidTill = @ValidTill,
		Reason = @Reason
	WHERE ID = @CaseID
END