
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_GetPaymentTransactionsForInvoiceId]
	@AccountTypeID uniqueidentifier,
	@InvoiceID bigint

AS
BEGIN
	SELECT	bt.[ID],bt.AccountID,bt.AccountTypeID, TransactionTypeID,Amount,bt.CurrencyId,TransactionTime,Notes,Reference,IsBalanceUpdated,ClosingPeriodId, SourceID, Settings, bt.IsDeleted, IsSubtractedFromBalance, bt.PaymentToInvoiceID
	FROM VR_AccountBalance.BillingTransaction bt with(nolock)
	
	where 
		bt.AccountTypeID = @AccountTypeID
		AND isnull(bt.IsDeleted, 0) = 0
		AND bt.PaymentToInvoiceID = @InvoiceID
		  
END