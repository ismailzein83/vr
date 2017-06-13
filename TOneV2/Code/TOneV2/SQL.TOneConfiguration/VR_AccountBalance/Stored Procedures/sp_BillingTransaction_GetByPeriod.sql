CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_GetByPeriod]
	@AccountTypeIds varchar(max) = NULL,
	@AccountsIds varchar(max) = NULL,
	@TransactionTypeIds varchar(max),
	@FromDate datetime,
	@ToDate datetime 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	    BEGIN
	    DECLARE @AccountsIdsTable TABLE (AccountID varchar(50))
		INSERT INTO @AccountsIdsTable (AccountID)
		select ParsedString from [VR_AccountBalance].[ParseStringList](@AccountsIds)

		DECLARE @TransactionTypeIdsTable TABLE (TransactionTypeID uniqueidentifier)
		INSERT INTO @TransactionTypeIdsTable (TransactionTypeID)
		select Convert(uniqueidentifier, ParsedString) from [VR_AccountBalance].[ParseStringList](@TransactionTypeIds)

		DECLARE @AccountTypeIdsTable TABLE (AccountTypeID uniqueidentifier)
		INSERT INTO @AccountTypeIdsTable (AccountTypeID)
		select Convert(uniqueidentifier, ParsedString) from [VR_AccountBalance].[ParseStringList](@AccountTypeIds)


		SELECT		bt.[ID],bt.AccountID,bt.AccountTypeID, bt.TransactionTypeID,bt.Amount,bt.CurrencyId,bt.TransactionTime,bt.Notes,bt.Reference,bt.IsBalanceUpdated,bt.ClosingPeriodId, SourceID, bt.Settings, bt.IsDeleted, bt.IsSubtractedFromBalance
		FROM	[VR_AccountBalance].BillingTransaction bt  with(nolock)
		
        WHERE	isnull(bt.IsDeleted, 0) = 0
				and (@AccountsIds  IS NULL or bt.AccountID IN (select AccountID from @AccountsIdsTable))
			    AND (@TransactionTypeIds  IS NULL OR bt.TransactionTypeID IN (select TransactionTypeID from @TransactionTypeIdsTable))
				AND (@AccountTypeIds  IS NULL OR bt.AccountTypeID IN (select AccountTypeID from @AccountTypeIdsTable))
				AND (ISNULL(bt.IsDeleted,0)=0)
				AND TransactionTime >= @FromDate
				AND (@ToDate IS NULL OR  TransactionTime <= @ToDate)

		
		END
	SET NOCOUNT OFF
END