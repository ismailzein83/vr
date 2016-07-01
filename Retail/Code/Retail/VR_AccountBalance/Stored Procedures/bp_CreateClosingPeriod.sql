-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE VR_AccountBalance.bp_CreateClosingPeriod 
	@ClosingTime datetime,
	@UsageTransactionTypeId int
AS
BEGIN
DECLARE @ClosingPeriodID bigint

BEGIN TRY
	BEGIN TRANSACTION 
	
		INSERT INTO [VR_AccountBalance].[BalanceClosingPeriod]
           ([ClosingTime])
		 VALUES
		   (@ClosingTime)
		
		SET @ClosingPeriodID = @@identity
		
		INSERT INTO [VR_AccountBalance].[BillingTransaction]
				   ([AccountID]
				   ,[TransactionTypeID]
				   ,[Amount]
				   ,[CurrencyId]
				   ,[TransactionTime]
				   ,[IsBalanceUpdated]
				   ,[ClosingPeriodId])
		SELECT [AccountID]
			   ,@UsageTransactionTypeId
			   ,[UsageBalance]
			   ,[CurrencyID]
			   ,@ClosingTime
			   ,1
			   ,@ClosingPeriodID
		FROM [VR_AccountBalance].[LiveBalance]
		
		INSERT INTO [VR_AccountBalance].[BalanceHistory]
				   ([ClosingPeriodID]
				   ,[AccountID]
				   ,[ClosingBalance])
		SELECT @ClosingPeriodID
			   ,[AccountID]
			   ,CurrentBalance
		FROM [VR_AccountBalance].[LiveBalance]
		
		UPDATE [VR_AccountBalance].[LiveBalance]
		SET InitialBalance = CurrentBalance,
			UsageBalance = 0
			
		UPDATE [VR_AccountBalance].[BillingTransaction]
		SET ClosingPeriodId = @ClosingPeriodID
		WHERE @ClosingPeriodID IS NULL AND ISNULL(IsBalanceUpdated, 0) = 1

	COMMIT ;
END TRY
BEGIN CATCH

IF @@TRANCOUNT > 0
    ROLLBACK;
END CATCH

END