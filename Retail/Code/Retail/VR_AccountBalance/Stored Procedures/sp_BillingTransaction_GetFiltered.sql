CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_GetFiltered]
	@AccountsIds varchar(max),
	@TransactionTypeIds varchar(max),
	@AccountTypeID uniqueidentifier,
	@FromTime datetime,
	@ToTime datetime = null
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

		SELECT	bt.[ID],bt.AccountID,bt.AccountTypeID, bt.TransactionTypeID,bt.Amount,bt.CurrencyId,bt.TransactionTime,bt.Notes,bt.Reference,bt.IsBalanceUpdated,bt.ClosingPeriodId
		FROM	[VR_AccountBalance].BillingTransaction bt  with(nolock)
		
        WHERE	
				 (@AccountsIds  is null or bt.AccountID in (select AccountID from @AccountsIdsTable))
				 and  (@TransactionTypeIds  is null or bt.TransactionTypeID in (select TransactionTypeID from @TransactionTypeIdsTable))
				 and (bt.TransactionTime >= @FromTime)
				 and (@ToTime is null or bt.TransactionTime <= @ToTime)
				 and (ISNULL(bt.IsDeleted,0)=0)
				 and bt.AccountTypeID = @AccountTypeID
		END
	SET NOCOUNT OFF
END