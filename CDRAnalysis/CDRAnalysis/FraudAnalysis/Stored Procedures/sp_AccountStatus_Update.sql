
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountStatus_Update]
	@AccountNumber VARCHAR(50),
	@ValidTill DATETIME, 
	@Source int,
	@Reason varchar(max),
	@UserId int
AS
BEGIN
	UPDATE FraudAnalysis.AccountStatus
	SET  ValidTill = @ValidTill, Source=@Source, Reason=@Reason, UserId=@UserId ,LastUpdatedOn = getdate()
	WHERE AccountNumber = @AccountNumber
END