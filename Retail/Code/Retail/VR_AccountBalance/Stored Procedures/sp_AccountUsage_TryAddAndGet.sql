-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_AccountUsage_TryAddAndGet]
	@AccountTypeID uniqueidentifier,
	@AccountID bigint,
	@PeriodStart datetime,
	@PeriodEnd datetime,
	@CurrencyId int,
	@UsageBalance decimal(20,6),
	@BillingTransactionNote nvarchar(1000)
AS
BEGIN

	Declare @ID bigint;

	Select @ID = ID from [VR_AccountBalance].AccountUsage WHERE AccountID = @AccountID AND AccountTypeID = @AccountTypeID AND PeriodStart =@PeriodStart
	IF(@ID IS NULL)
	BEGIN
		INSERT INTO [VR_AccountBalance].AccountUsage (AccountTypeID,AccountID ,CurrencyId, PeriodStart, PeriodEnd, UsageBalance,BillingTransactionNote)
		VALUES (@AccountTypeID, @AccountID,@CurrencyId, @PeriodStart, @PeriodEnd, @UsageBalance,@BillingTransactionNote)	
		Set @ID = SCOPE_IDENTITY()
	END
	SELECT @ID as ID, @AccountID as AccountID
END