-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_GetById]
	@BillingTransactionId bigint
AS
BEGIN
	SELECT TOP(1) bt.[ID],bt.AccountID,bt.AccountTypeID, TransactionTypeID,Amount,bt.CurrencyId,TransactionTime,Notes,Reference,IsBalanceUpdated,ClosingPeriodId, SourceID, Settings, bt.IsDeleted, IsSubtractedFromBalance
	FROM VR_AccountBalance.BillingTransaction bt WITH(NOLOCK)
	WHERE ISNULL(bt.IsDeleted, 0) = 0
		  AND bt.ID = @BillingTransactionId
END