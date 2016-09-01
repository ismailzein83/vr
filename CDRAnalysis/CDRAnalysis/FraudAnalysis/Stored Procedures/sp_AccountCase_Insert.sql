-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_Insert]
	@CaseId int,
	@AccountNumber VARCHAR(50),
	@UserID INT,
	@CaseStatusID INT,
	@ValidTill DATETIME,
	@Reason VARCHAR(MAX) = NULL
AS
BEGIN
	INSERT INTO FraudAnalysis.AccountCase (ID, AccountNumber, UserID, [Status], StatusUpdatedTime, ValidTill, CreatedTime, Reason)
	VALUES (@CaseId, @AccountNumber, @UserID, @CaseStatusID, GETDATE(), @ValidTill, GETDATE(), @Reason)
END