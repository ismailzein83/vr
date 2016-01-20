-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [FraudAnalysis].[sp_AccountInfo_InsertOrUpdate]
	@AccountNumber VARCHAR(50),
	@InfoDetail VARCHAR(Max)
AS
BEGIN
	UPDATE AccountInfo SET  InfoDetail=@InfoDetail WHERE AccountNumber = @AccountNumber
	
	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO AccountInfo (AccountNumber, InfoDetail) VALUES (@AccountNumber, @InfoDetail)
	END
END