-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_AccountUsage_GetForSpecificPeriodByAccountIds]
	@AccountTypeID uniqueidentifier,
	@TransactionTypeId uniqueidentifier,
	@DatePeriod Datetime,
	@AccountIds varchar(MAX)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @AccountIdsTable TABLE (AccountId varchar(50))
	INSERT INTO @AccountIdsTable (AccountId)
	SELECT ParsedString FROM [VR_AccountBalance].[ParseStringList](@AccountIds)

	SELECT  au.ID,
			au.AccountTypeID,
			au.AccountID,
			au.BillingTransactionID,
			au.BillingTransactionNote,
			au.CurrencyId,
			au.PeriodEnd,
			au.PeriodStart,
			au.ShouldRecreateTransaction,
			au.TransactionTypeID,
			au.UsageBalance
FROM	VR_AccountBalance.AccountUsage au WITH(NOLOCK) 
WHERE	AccountID in (SELECT AccountId FROM @AccountIdsTable)
AND AccountTypeID = @AccountTypeID AND PeriodStart = @DatePeriod AND TransactionTypeId= @TransactionTypeId
END