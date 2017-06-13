-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE  PROCEDURE [VR_AccountBalance].[sp_AccountUsage_GetForFilteredForBillingTransaction]
	@AccountTypeID uniqueidentifier,
	@TransactionTypeIds varchar(MAX),
	@AccountIds varchar(MAX),
	@FromTime Datetime,
	@ToTime Datetime = NULL
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @AccountIdsTable TABLE (AccountId varchar(50))
	INSERT INTO @AccountIdsTable (AccountId)
	SELECT ParsedString FROM [VR_AccountBalance].[ParseStringList](@AccountIds)

	DECLARE @TransactionTypeIdsTable TABLE (TransactionTypeId uniqueidentifier)
	INSERT INTO @TransactionTypeIdsTable (TransactionTypeId)
	SELECT ParsedString FROM [VR_AccountBalance].[ParseStringList](@TransactionTypeIds)

	SELECT  au.ID,
			au.AccountTypeID,
			au.AccountID,
			au.CurrencyId,
			au.PeriodEnd,
			au.PeriodStart,
			au.TransactionTypeID,
			au.UsageBalance,
			au.IsOverridden,
			au.OverriddenAmount,
			au.CorrectionProcessID
FROM	VR_AccountBalance.AccountUsage au WITH(NOLOCK) 
WHERE	isnull(au.IsOverridden, 0) = 0
		and (@AccountIds  IS NULL OR AccountID in (SELECT AccountId FROM @AccountIdsTable))
		AND (@TransactionTypeIds  IS NULL OR TransactionTypeID in (SELECT TransactionTypeId FROM @TransactionTypeIdsTable))
		AND AccountTypeID = @AccountTypeID 
		AND PeriodStart >= @FromTime
		AND (@ToTime IS NULL OR PeriodStart <= @ToTime)
	
END