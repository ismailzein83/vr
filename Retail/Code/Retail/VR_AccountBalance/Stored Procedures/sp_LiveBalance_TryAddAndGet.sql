-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].sp_LiveBalance_TryAddAndGet
	@AccountID bigint,
	@AccountTypeID uniqueidentifier,
	@InitialBalance decimal(20,6),
	@CurrencyId int,
	@UsageBalance decimal(20,6),
	@CurrentBalance decimal(20,6)
	
AS
BEGIN

	Declare @ID bigint;

	Select @ID = ID from [VR_AccountBalance].LiveBalance WHERE AccountID = @AccountID AND AccountTypeID = @AccountTypeID
	IF(@ID IS NULL)
	BEGIN
		INSERT INTO [VR_AccountBalance].LiveBalance (AccountID , AccountTypeID, InitialBalance, CurrencyID, UsageBalance,CurrentBalance)
		VALUES (@AccountID, @AccountTypeID, @InitialBalance, @CurrencyId, @UsageBalance,@CurrentBalance)	
		Set @ID = SCOPE_IDENTITY()
	END
	SELECT @ID as ID, @CurrencyId as CurrencyId ,@AccountID as AccountID
END