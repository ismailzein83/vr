
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountStatus_Insert]
	@AccountNumber VARCHAR(50),
	@StatusID INT,
	@ValidTill DATETIME, 
	@Source int,
	@Reason varchar(max),
	@UserId int
AS
BEGIN
	IF NOT Exists (SELECT null FROM FraudAnalysis.[AccountStatus] WHERE AccountNumber = @AccountNumber)
	BEGIN
		Insert into FraudAnalysis.[AccountStatus](AccountNumber, [Status], ValidTill, Source, Reason, UserId,LastUpdatedOn)
		values(@AccountNumber, @StatusID,  @ValidTill, @Source, @Reason, @UserId,getdate())
	END
END