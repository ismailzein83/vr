-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_Insert]
	@AccountID bigint,
	@InitialBalance decimal(20,6),
	@CurrencyId int,
	@UsageBalance decimal(20,6),
	@CurrentBalance decimal(20,6)
	
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM [VR_AccountBalance].LiveBalance WHERE AccountID = @AccountID)
	BEGIN
		INSERT INTO [VR_AccountBalance].LiveBalance (AccountID,InitialBalance, CurrencyID, UsageBalance,CurrentBalance)
		VALUES (@AccountID,@InitialBalance, @CurrencyId, @UsageBalance,@CurrentBalance)
	END
END