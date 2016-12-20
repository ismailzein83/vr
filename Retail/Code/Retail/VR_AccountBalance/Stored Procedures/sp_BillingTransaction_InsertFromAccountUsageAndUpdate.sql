-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_InsertFromAccountUsageAndUpdate]
	@AccountTypeID uniqueidentifier,
	@TransactionTypeID uniqueidentifier
AS
BEGIN
	DECLARE @AccountUsageId bigint,@AccountID bigint, @PeriodStart datetime,@PeriodEnd datetime,@UsageBalance decimal(20,6),@CurrencyID int,@BillingTrasactionNote nvarchar(1000)
	
	DECLARE db_cursor CURSOR FOR  
	SELECT ID,AccountID,PeriodStart,PeriodEnd,UsageBalance,CurrencyId,BillingTransactionNote FROM [VR_AccountBalance].AccountUsage
	WHERE AccountTypeID =  @AccountTypeID AND PeriodEnd<GETDATE() AND BillingTransactionID IS NULL

	OPEN db_cursor   
	FETCH NEXT FROM db_cursor INTO @AccountUsageId,@AccountID, @PeriodStart,  @PeriodEnd,@UsageBalance,@CurrencyID,@BillingTrasactionNote

	WHILE @@FETCH_STATUS = 0   
		BEGIN   
			DECLARE @ID bigint;
			INSERT INTO VR_AccountBalance.BillingTransaction (AccountTypeID,AccountID,TransactionTypeID, Amount, CurrencyId,TransactionTime,Notes,IsBalanceUpdated)
			VALUES (@AccountTypeID,@AccountID, @TransactionTypeID,@UsageBalance, @CurrencyId,@PeriodEnd, @BillingTrasactionNote,1)
			SET @ID = SCOPE_IDENTITY()
			Update [VR_AccountBalance].AccountUsage set BillingTransactionID =@ID where ID = @AccountUsageId 
			FETCH NEXT FROM db_cursor INTO @AccountUsageId,@AccountID, @PeriodStart,  @PeriodEnd,@UsageBalance,@CurrencyID,@BillingTrasactionNote
		END   
	CLOSE db_cursor   
	DEALLOCATE db_cursor
END