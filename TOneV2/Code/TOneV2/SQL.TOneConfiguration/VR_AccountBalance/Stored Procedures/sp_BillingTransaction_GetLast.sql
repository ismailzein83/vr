-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_GetLast]
	@AccountTypeID uniqueidentifier,
	@AccountId nvarchar(255)
AS
BEGIN
	SELECT TOP(1) bt.[ID],bt.AccountID,bt.AccountTypeID, TransactionTypeID,Amount,bt.CurrencyId,TransactionTime,Notes,Reference,IsBalanceUpdated,ClosingPeriodId, SourceID, Settings, bt.IsDeleted, IsSubtractedFromBalance
	FROM VR_AccountBalance.BillingTransaction bt WITH(NOLOCK)
	WHERE ISNULL(bt.IsDeleted, 0) = 0
		  AND bt.AccountTypeID = @AccountTypeID 
		  AND bt.AccountId= @AccountId
	ORDER BY TransactionTime DESC
END