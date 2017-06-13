CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_GetByTime]
	@BillingTransactionsByTimeTable VR_AccountBalance.[BillingTransactionsByTimeTable] READONLY,
	@AccountTypeId uniqueidentifier,
	@TransactionTypeIds varchar(max)
AS
BEGIN
SET NOCOUNT ON;
	DECLARE @TransactionTypeIdsTable TABLE (TransactionTypeId uniqueidentifier)
	INSERT INTO @TransactionTypeIdsTable (TransactionTypeId)
	SELECT CONVERT(uniqueidentifier, ParsedString) FROM [VR_Invoice].[ParseStringList](@TransactionTypeIds)
	
	SELECT	bt.AccountID,bt.Amount,bt.CurrencyId,bt.TransactionTime,bt.TransactionTypeID
	FROM	VR_AccountBalance.BillingTransaction bt with(nolock)
	JOIN	@BillingTransactionsByTimeTable btt on bt.AccountID = btt.AccountID AND bt.TransactionTime > btt.TransactionTime
	WHERE	isnull(bt.IsDeleted, 0) = 0
			and bt.AccountTypeId = @AccountTypeId AND (@TransactionTypeIds IS NULL OR bt.TransactionTypeID IN (SELECT TransactionTypeId FROM @TransactionTypeIdsTable))
End