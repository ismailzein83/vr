
CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_GetForSynchronizerProcess]
	@TransactionTypeIds varchar(max),
	@AccountTypeID uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;
	
	    BEGIN

		DECLARE @TransactionTypeIdsTable TABLE (TransactionTypeID uniqueidentifier)
		INSERT INTO @TransactionTypeIdsTable (TransactionTypeID)
		select Convert(uniqueidentifier, ParsedString) from [VR_AccountBalance].[ParseStringList](@TransactionTypeIds)

		SELECT	bt.[ID],bt.AccountID,bt.AccountTypeID, bt.TransactionTypeID,bt.Amount,bt.CurrencyId,bt.TransactionTime,bt.Notes,bt.Reference,bt.IsBalanceUpdated,bt.ClosingPeriodId, bt.SourceID
		FROM	[VR_AccountBalance].BillingTransaction bt  with(nolock)
		
        WHERE	SourceId is not null	
				and (@TransactionTypeIds  is null or bt.TransactionTypeID in (select TransactionTypeID from @TransactionTypeIdsTable))
				and bt.AccountTypeID = @AccountTypeID
		END
	SET NOCOUNT OFF
END