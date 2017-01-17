-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_AccountUsage_TryAddAndGet]
	@AccountTypeID uniqueidentifier,
	@TransactionTypeID uniqueidentifier,
	@AccountID bigint,
	@PeriodStart datetime,
	@PeriodEnd datetime,
	@CurrencyId int,
	@UsageBalance decimal(20,6),
	@BillingTransactionNote nvarchar(1000)
AS
BEGIN

	Declare @ID bigint;

	Select @ID = ID from [VR_AccountBalance].AccountUsage WHERE AccountID = @AccountID AND AccountTypeID = @AccountTypeID AND TransactionTypeID = @TransactionTypeID  AND  PeriodStart =@PeriodStart
	IF(@ID IS NULL)
	BEGIN
		INSERT INTO [VR_AccountBalance].AccountUsage (AccountTypeID,TransactionTypeID,AccountID ,CurrencyId, PeriodStart, PeriodEnd, UsageBalance,BillingTransactionNote)
		VALUES (@AccountTypeID,@TransactionTypeID, @AccountID,@CurrencyId, @PeriodStart, @PeriodEnd, @UsageBalance,@BillingTransactionNote)	
		Set @ID = SCOPE_IDENTITY()
	END
	SELECT @ID as ID, @AccountID as AccountID,@TransactionTypeID as TransactionTypeID
END