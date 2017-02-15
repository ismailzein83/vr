CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_GetBalanceNotUpdated]
	@AccountTypeID uniqueidentifier
AS
BEGIN
SELECT	bt.[ID],bt.AccountID,bt.AccountTypeID,bt.TransactionTypeID,bt.Amount,bt.CurrencyId,bt.TransactionTime,bt.Notes,bt.Reference,bt.IsBalanceUpdated,bt.ClosingPeriodId, bt.SourceID
FROM	[VR_AccountBalance].BillingTransaction bt  with(nolock)
WHERE	ISNULL(IsBalanceUpdated, 0) = 0 and bt.AccountTypeID = @AccountTypeID 
order by AccountID
END