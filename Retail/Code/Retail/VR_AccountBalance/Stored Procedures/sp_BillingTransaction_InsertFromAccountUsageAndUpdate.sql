-- =============================================
-- Author:  	<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_InsertFromAccountUsageAndUpdate] @AccountTypeID uniqueidentifier,
@TimeOffset decimal
AS
BEGIN

  DECLARE  @BeforeDate DATETIME
  SET @BeforeDate =  DATEADD(SECOND, -@TimeOffset,GETDATE())

  DECLARE @AccountUsageId bigint,
          @AccountID bigint,
          @TransactionTypeID uniqueidentifier,
          @ExistingBillingTransactionId bigint,
          @PeriodStart datetime,
          @PeriodEnd datetime,
          @UsageBalance decimal(20, 6),
          @CurrencyID int,
          @BillingTrasactionNote nvarchar(1000)

  DECLARE db_cursor CURSOR FOR
  SELECT
    ID,
    AccountID,
    PeriodStart,
    PeriodEnd,
    UsageBalance,
    CurrencyId,
    BillingTransactionNote,
    TransactionTypeID,
    BillingTransactionId
  FROM [VR_AccountBalance].AccountUsage
  WHERE AccountTypeID = @AccountTypeID
  AND PeriodEnd < @BeforeDate
  AND (BillingTransactionID IS NULL
  OR ISNULL(ShouldRecreateTransaction, 0) = 1)

  OPEN db_cursor
  FETCH NEXT
  FROM db_cursor
  INTO @AccountUsageId,
  @AccountID,
  @PeriodStart,
  @PeriodEnd,
  @UsageBalance,
  @CurrencyID,
  @BillingTrasactionNote,
  @TransactionTypeID,
  @ExistingBillingTransactionId

  WHILE @@FETCH_STATUS = 0
  BEGIN
    BEGIN TRANSACTION InsertFromAccountUsageAndUpdate
      DECLARE @NewBillingTransactionId bigint;
      IF (@UsageBalance > 0)
      BEGIN
        INSERT INTO VR_AccountBalance.BillingTransaction (AccountTypeID, AccountID, TransactionTypeID, Amount, CurrencyId, TransactionTime, Notes, IsBalanceUpdated)
          VALUES (@AccountTypeID, @AccountID, @TransactionTypeID, @UsageBalance, @CurrencyId, @PeriodEnd, @BillingTrasactionNote, 1)
        SET @NewBillingTransactionId = SCOPE_IDENTITY()
      END

      IF (@ExistingBillingTransactionId IS NOT NULL)
      BEGIN
        UPDATE [VR_AccountBalance].BillingTransaction
        SET IsDeleted = 1
        WHERE ID = @ExistingBillingTransactionId
      END
      UPDATE [VR_AccountBalance].AccountUsage
      SET BillingTransactionID = @NewBillingTransactionId,
          ShouldRecreateTransaction = 0
      WHERE ID = @AccountUsageId


      FETCH NEXT
      FROM db_cursor
      INTO @AccountUsageId,
      @AccountID,
      @PeriodStart,
      @PeriodEnd,
      @UsageBalance,
      @CurrencyID,
      @BillingTrasactionNote,
      @TransactionTypeID,
      @ExistingBillingTransactionId
    COMMIT TRANSACTION InsertFromAccountUsageAndUpdate
  END
  CLOSE db_cursor
  DEALLOCATE db_cursor
END