
CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_GetTransactionsToProcess]
	@AccountTypeID uniqueidentifier
AS
BEGIN
	select [ID]
      ,[AccountTypeID]
      ,[AccountID]
      ,[TransactionTypeID]
      ,[Amount]
      ,[CurrencyId]
      ,[TransactionTime]
      ,[Reference]
      ,[Notes]
      ,[CreatedByInvoiceID]
      ,[IsBalanceUpdated]
      ,[ClosingPeriodId]
      ,[Settings]
      ,[IsDeleted]
      ,[IsSubtractedFromBalance]
      ,[SourceID]
	from [VR_AccountBalance].BillingTransaction with(nolock)
	where AccountTypeID = @AccountTypeID and (isnull(IsBalanceUpdated, 0) = 0 or (IsBalanceUpdated = 1 and IsDeleted = 1 and isnull(IsSubtractedFromBalance, 0) = 0))
	order by AccountID
END