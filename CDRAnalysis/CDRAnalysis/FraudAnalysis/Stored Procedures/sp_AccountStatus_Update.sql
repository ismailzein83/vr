
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountStatus_Update]
	@AccountNumber VARCHAR(50),
	@ValidTill DATETIME
AS
BEGIN
	UPDATE FraudAnalysis.AccountStatus
	SET  ValidTill = @ValidTill
	WHERE AccountNumber = @AccountNumber
END