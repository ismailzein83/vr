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
	@ToTime Datetime = NULL,
	@Status int = null,
	@EffectiveDate  datetime = null,
	@IsEffectiveInFuture bit = null
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
		LEFT JOIN [VR_AccountBalance].LiveBalance vrlb
	    on vrlb.AccountTypeID = au.AccountTypeID and 
		   vrlb.AccountID = au.AccountID 
WHERE	ISNULL(au.IsOverridden, 0) = 0
		and (@AccountIds  IS NULL OR au.AccountID in (SELECT AccountId FROM @AccountIdsTable))
		AND (@TransactionTypeIds  IS NULL OR TransactionTypeID in (SELECT TransactionTypeId FROM @TransactionTypeIdsTable))
		AND au.AccountTypeID = @AccountTypeID 
		AND PeriodStart >= @FromTime
		AND (@ToTime IS NULL OR PeriodStart <= @ToTime)
	    AND (vrlb.AccountID IS NULL 
		OR ((@Status IS NULL OR vrlb.[Status] = @Status) 
		AND (@EffectiveDate IS NULL OR ((vrlb.BED <= @EffectiveDate OR vrlb.BED IS NULL) 
		AND (vrlb.EED > @EffectiveDate OR vrlb.EED IS NULL))) 
		AND (@IsEffectiveInFuture IS NUll OR (@IsEffectiveInFuture = 1 and (vrlb.EED IS NULL or vrlb.EED >=  GETDATE())) 
		OR  (@IsEffectiveInFuture = 0 and  vrlb.EED <=  GETDATE()))))
END