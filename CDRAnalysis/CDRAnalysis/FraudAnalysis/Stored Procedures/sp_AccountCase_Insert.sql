-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_Insert]
	@AccountNumber VARCHAR(50),
	@UserID INT,
	@CaseStatusID INT,
	@ValidTill DATETIME,
	@InsertedID INT OUT
AS
BEGIN
	INSERT INTO FraudAnalysis.AccountCase (AccountNumber, UserID, [Status], StatusUpdatedTime, ValidTill, CreatedTime)
	VALUES (@AccountNumber, @UserID, @CaseStatusID, GETDATE(), @ValidTill, GETDATE())
	
	SET @InsertedID = @@IDENTITY
END