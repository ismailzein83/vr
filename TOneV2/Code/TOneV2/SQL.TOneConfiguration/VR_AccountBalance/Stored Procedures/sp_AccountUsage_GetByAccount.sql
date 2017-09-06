-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_AccountUsage_GetByAccount]
	@AccountTypeID uniqueidentifier,
	@AccountId nvarchar(255),
	@Status int = null,
	@EffectiveDate  datetime = null,
	@IsEffectiveInFuture bit
AS
BEGIN
		SELECT au.ID, au.AccountTypeID,TransactionTypeID, au.AccountID,au.CurrencyId,PeriodStart,PeriodEnd,UsageBalance, IsOverridden, OverriddenAmount, CorrectionProcessID
	FROM VR_AccountBalance.AccountUsage au with(nolock)
	Inner Join [VR_AccountBalance].LiveBalance vrlb
	    on vrlb.AccountTypeID = au.AccountTypeID and 
		   vrlb.AccountID = au.AccountID and 
	       ISNULL(vrlb.IsDeleted, 0) = 0 and
	       (@Status IS NULL OR vrlb.[Status] = @Status) AND
	       (@EffectiveDate IS NULL OR ((vrlb.BED <= @EffectiveDate OR vrlb.BED IS NULL)  AND (vrlb.EED > @EffectiveDate OR vrlb.EED IS NULL))) AND
	       (@IsEffectiveInFuture IS NUll OR (@IsEffectiveInFuture = 1 and (vrlb.EED IS NULL or vrlb.EED >=  GETDATE()))  OR  (@IsEffectiveInFuture = 0 and  vrlb.EED <=  GETDATE()))

	where au.AccountTypeID = @AccountTypeID 
		  AND au.AccountId= @AccountId
		  and isnull(IsOverridden, 0) = 0
END