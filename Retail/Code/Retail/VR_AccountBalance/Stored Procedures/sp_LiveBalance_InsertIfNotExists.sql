-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_InsertIfNotExists]
	@AccountID bigint,
	@AccountTypeID uniqueidentifier,
	@InitialBalance decimal(20,6),
	@CurrencyId int,
	@UsageBalance decimal(20,6),
	@CurrentBalance decimal(20,6)
	
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM [VR_AccountBalance].LiveBalance WITH (NOLOCK) WHERE AccountTypeID = @AccountTypeID AND AccountID = @AccountID)
	BEGIN
		INSERT INTO [VR_AccountBalance].LiveBalance (AccountID , AccountTypeID, InitialBalance, CurrencyID, UsageBalance,CurrentBalance)
		VALUES (@AccountID, @AccountTypeID, @InitialBalance, @CurrencyId, @UsageBalance,@CurrentBalance)		
	END
END