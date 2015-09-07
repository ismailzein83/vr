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
	DECLARE @Found INT = (SELECT COUNT(*) FROM AccountStatus WHERE AccountNumber = @AccountNumber)
	
	IF @Found = 0
		BEGIN
			INSERT INTO AccountStatus (AccountNumber, [Status]) VALUES (@AccountNumber, @StatusID)
		END
	ELSE
		BEGIN
			UPDATE AccountStatus SET [Status] = @StatusID WHERE AccountNumber = @AccountNumber
		END
END