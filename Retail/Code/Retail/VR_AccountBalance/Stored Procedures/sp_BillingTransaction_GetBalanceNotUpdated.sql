CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_GetBalanceNotUpdated]

AS
BEGIN
SELECT	bt.[ID],bt.AccountID,bt.TransactionTypeID,bt.Amount,bt.CurrencyId,bt.TransactionTime,bt.Notes,bt.Reference,bt.IsBalanceUpdated,bt.ClosingPeriodId
FROM	[VR_AccountBalance].BillingTransaction bt  with(nolock)
WHERE	ISNULL(IsBalanceUpdated, 0) = 0 order by AccountID
END