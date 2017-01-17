-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_InsertFromAccountUsageAndUpdate]
	@AccountTypeID uniqueidentifier,
	@TimeOffset time
AS
BEGIN
	DECLARE @AccountUsageId bigint,@AccountID bigint,@TransactionTypeID uniqueidentifier,@BillingTransactionId bigint, @PeriodStart datetime,@PeriodEnd datetime,@UsageBalance decimal(20,6),@CurrencyID int,@BillingTrasactionNote nvarchar(1000)
	
	DECLARE db_cursor CURSOR FOR  
	SELECT ID,AccountID,PeriodStart,PeriodEnd,UsageBalance,CurrencyId,BillingTransactionNote,TransactionTypeID,BillingTransactionId FROM [VR_AccountBalance].AccountUsage
	WHERE AccountTypeID =  @AccountTypeID AND PeriodEnd< GETDATE() - @TimeOffset AND (BillingTransactionID IS NULL OR ShouldRecreateTransaction IS NOT NULL)

	OPEN db_cursor   
	FETCH NEXT FROM db_cursor INTO @AccountUsageId,@AccountID, @PeriodStart,  @PeriodEnd,@UsageBalance,@CurrencyID,@BillingTrasactionNote,@TransactionTypeID,@BillingTransactionId

	WHILE @@FETCH_STATUS = 0   
		BEGIN   
		 BEGIN TRANSACTION InsertFromAccountUsageAndUpdate  
			DECLARE @ID bigint;
			INSERT INTO VR_AccountBalance.BillingTransaction (AccountTypeID,AccountID,TransactionTypeID, Amount, CurrencyId,TransactionTime,Notes,IsBalanceUpdated)
			VALUES (@AccountTypeID,@AccountID, @TransactionTypeID,@UsageBalance, @CurrencyId,@PeriodEnd, @BillingTrasactionNote,1)
			SET @ID = SCOPE_IDENTITY()

			IF (ISNULL(@BillingTransactionId,0) > 0)
			BEGIN
				Update [VR_AccountBalance].BillingTransaction set IsDeleted = 1 where ID =  @BillingTransactionId
			END
			Update [VR_AccountBalance].AccountUsage set BillingTransactionID =@ID , ShouldRecreateTransaction = null where ID = @AccountUsageId 
			FETCH NEXT FROM db_cursor INTO @AccountUsageId,@AccountID, @PeriodStart,  @PeriodEnd,@UsageBalance,@CurrencyID,@BillingTrasactionNote,@TransactionTypeID,@BillingTransactionId
			COMMIT TRANSACTION InsertFromAccountUsageAndUpdate
		END   
	CLOSE db_cursor   
	DEALLOCATE db_cursor
END