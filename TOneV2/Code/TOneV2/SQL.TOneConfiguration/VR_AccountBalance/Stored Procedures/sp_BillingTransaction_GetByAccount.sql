-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_GetByAccount]
	@AccountTypeID uniqueidentifier,
	@AccountId nvarchar(255),
	@Status int = null,
	@EffectiveDate  datetime = null,
	@IsEffectiveInFuture bit

AS
BEGIN
	SELECT	bt.[ID],bt.AccountID,bt.AccountTypeID, TransactionTypeID,Amount,bt.CurrencyId,TransactionTime,Notes,Reference,IsBalanceUpdated,ClosingPeriodId, SourceID, Settings, bt.IsDeleted, IsSubtractedFromBalance
	FROM VR_AccountBalance.BillingTransaction bt with(nolock)
	Inner Join [VR_AccountBalance].LiveBalance vrlb
	    on vrlb.AccountTypeID = bt.AccountTypeID and 
		   vrlb.AccountID = bt.AccountID and 
	       ISNULL(vrlb.IsDeleted, 0) = 0 and
	       (@Status IS NULL OR vrlb.[Status] = @Status) AND
	       (@EffectiveDate IS NULL OR ((vrlb.BED <= @EffectiveDate OR vrlb.BED IS NULL) AND (vrlb.EED > @EffectiveDate OR vrlb.EED IS NULL))) AND
	       (@IsEffectiveInFuture IS NUll OR (@IsEffectiveInFuture = 1 and (vrlb.EED IS NULL or vrlb.EED >=  GETDATE()))  OR  (@IsEffectiveInFuture = 0 and  vrlb.EED <=  GETDATE()))

	where isnull(bt.IsDeleted, 0) = 0
		and bt.AccountTypeID = @AccountTypeID 
		AND bt.AccountId= @AccountId
		  
END