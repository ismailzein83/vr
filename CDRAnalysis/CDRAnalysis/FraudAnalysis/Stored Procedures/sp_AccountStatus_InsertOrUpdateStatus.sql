-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountStatus_InsertOrUpdateStatus]
	@AccountNumber VARCHAR(50),
	@StatusID INT,
	@ValidTill DATETIME = NULL
AS
BEGIN
	UPDATE AccountStatus SET [Status] = @StatusID, ValidTill = @ValidTill WHERE AccountNumber = @AccountNumber
	
	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO AccountStatus (AccountNumber, [Status]) VALUES (@AccountNumber, @StatusID)
	END
END