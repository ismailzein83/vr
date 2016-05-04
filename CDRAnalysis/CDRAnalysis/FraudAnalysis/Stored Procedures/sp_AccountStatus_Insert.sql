
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountStatus_Insert]
	@AccountNumber VARCHAR(50),
	@StatusID INT,
	@ValidTill DATETIME
AS
BEGIN
	IF NOT Exists (SELECT null FROM FraudAnalysis.[AccountStatus] WHERE AccountNumber = @AccountNumber)
	BEGIN
		Insert into FraudAnalysis.[AccountStatus](AccountNumber, [Status], ValidTill)
		values(@AccountNumber, @StatusID,  @ValidTill)
	END
END