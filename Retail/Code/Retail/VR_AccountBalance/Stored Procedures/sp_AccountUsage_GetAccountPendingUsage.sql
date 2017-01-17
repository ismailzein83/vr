-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_AccountUsage_GetAccountPendingUsage]
	@AccountTypeID uniqueidentifier,
	@AccountId bigint
AS
BEGIN
	SELECT ID, AccountTypeID,TransactionTypeID, AccountID,CurrencyId,PeriodStart,PeriodEnd,UsageBalance,BillingTransactionNote,BillingTransactionID,ShouldRecreateTransaction
	FROM VR_AccountBalance.AccountUsage with(nolock)
	where AccountTypeID = @AccountTypeID AND AccountID = @AccountId AND BillingTransactionID IS NULL
END