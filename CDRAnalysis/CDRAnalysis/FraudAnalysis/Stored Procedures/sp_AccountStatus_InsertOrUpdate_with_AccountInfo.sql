-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [FraudAnalysis].[sp_AccountStatus_InsertOrUpdate_with_AccountInfo]
	@AccountNumber VARCHAR(50),
	@StatusID INT,
	@AccountInfo VARCHAR(Max)
AS
BEGIN
	UPDATE AccountStatus SET [Status] = @StatusID, AccountInfo=@AccountInfo WHERE AccountNumber = @AccountNumber
	
	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO AccountStatus (AccountNumber, [Status], AccountInfo) VALUES (@AccountNumber, @StatusID, @AccountInfo)
	END
END