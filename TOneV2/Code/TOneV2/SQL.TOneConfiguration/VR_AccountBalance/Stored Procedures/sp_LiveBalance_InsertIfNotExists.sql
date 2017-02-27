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
	@CurrentBalance decimal(20,6)
	
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM [VR_AccountBalance].LiveBalance WITH (NOLOCK) WHERE AccountTypeID = @AccountTypeID AND AccountID = @AccountID)
	BEGIN
		INSERT INTO [VR_AccountBalance].LiveBalance (AccountID , AccountTypeID, InitialBalance, CurrencyID, CurrentBalance)
		VALUES (@AccountID, @AccountTypeID, @InitialBalance, @CurrencyId, @CurrentBalance)		
	END
END