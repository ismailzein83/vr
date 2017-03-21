CREATE PROCEDURE [VR_AccountBalance].sp_BillingTransaction_GetByAccountIds
	@AccountTypeID uniqueidentifier,
	@AccountsIds varchar(max) = NULL,
	@TransactionTypeIds varchar(max)
	
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

		SELECT	bt.AccountID,bt.TransactionTypeID,bt.CurrencyId,bt.Amount,bt.TransactionTime
		FROM	[VR_AccountBalance].BillingTransaction bt  with(nolock)
		
        WHERE	
				(@AccountsIds  IS NULL or bt.AccountID IN (select AccountID from @AccountsIdsTable))
			    AND (@TransactionTypeIds  IS NULL OR bt.TransactionTypeID IN (select TransactionTypeID from @TransactionTypeIdsTable))
				AND (ISNULL(bt.IsDeleted,0)=0)
				AND bt.AccountTypeID = @AccountTypeID

		
		END
	SET NOCOUNT OFF
END