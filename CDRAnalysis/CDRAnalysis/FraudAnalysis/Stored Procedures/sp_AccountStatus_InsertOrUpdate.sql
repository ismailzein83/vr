-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountStatus_InsertOrUpdate]
	@AccountNumber VARCHAR(50),
	@StatusID INT
AS
BEGIN
	UPDATE AccountStatus SET [Status] = @StatusID WHERE AccountNumber = @AccountNumber
	
	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO AccountStatus (AccountNumber, [Status]) VALUES (@AccountNumber, @StatusID)
	END
END