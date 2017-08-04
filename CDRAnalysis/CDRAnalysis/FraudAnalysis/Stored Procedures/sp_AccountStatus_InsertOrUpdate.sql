-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountStatus_InsertOrUpdate]
	@AccountNumber VARCHAR(50),
	@StatusID INT,
	@ValidTill DATETIME = NULL, 
	@Reason varchar(max),
	@Source int,
	@UserId int
AS
BEGIN
	UPDATE AccountStatus SET [Status] = @StatusID, ValidTill = @ValidTill, Source=@Source, Reason=@Reason, UserId=@UserId  WHERE AccountNumber = @AccountNumber
	
	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO AccountStatus (AccountNumber, [Status], ValidTill, Source, Reason, UserId,LastUpdatedOn) VALUES (@AccountNumber, @StatusID, @ValidTill , @Source, @Reason, @UserId,getdate())
	END
END