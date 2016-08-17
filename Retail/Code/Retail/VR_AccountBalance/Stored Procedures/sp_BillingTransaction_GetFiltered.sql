CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_GetFiltered]
	@AccountsIds varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	    BEGIN
	    DECLARE @AccountsIdsTable TABLE (AccountID int)
		INSERT INTO @AccountsIdsTable (AccountID)
		select Convert(int, ParsedString) from [VR_AccountBalance].[ParseStringList](@AccountsIds)
		SELECT	bt.[ID],bt.AccountID,bt.TransactionTypeID,bt.Amount,bt.CurrencyId,bt.TransactionTime,bt.Notes,bt.Reference,bt.IsBalanceUpdated,bt.ClosingPeriodId
		FROM	[VR_AccountBalance].BillingTransaction bt  with(nolock)
        WHERE	(@AccountsIds  is null or bt.AccountID in (select AccountID from @AccountsIdsTable))
		END
	SET NOCOUNT OFF
END