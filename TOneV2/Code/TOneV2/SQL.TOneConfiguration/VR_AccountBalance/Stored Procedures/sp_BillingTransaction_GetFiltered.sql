﻿CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_GetFiltered]
	@AccountsIds varchar(max),
	@TransactionTypeIds varchar(max),
	@AccountTypeID uniqueidentifier,
	@FromTime datetime,
	@ToTime datetime = null,
	@Status int = null,
	@EffectiveDate  datetime = null,
	@IsEffectiveInFuture bit = null
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	    BEGIN
	    DECLARE @AccountsIdsTable TABLE (AccountID varchar(50))
		INSERT INTO @AccountsIdsTable (AccountID)
		select ParsedString from [VR_AccountBalance].[ParseStringList](@AccountsIds)

		 DECLARE @TransactionTypeIdsTable TABLE (TransactionTypeID uniqueidentifier)
		INSERT INTO @TransactionTypeIdsTable (TransactionTypeID)
		select Convert(uniqueidentifier, ParsedString) from [VR_AccountBalance].[ParseStringList](@TransactionTypeIds)

		SELECT	bt.[ID],bt.AccountID,bt.AccountTypeID, bt.TransactionTypeID,bt.Amount,bt.CurrencyId,bt.TransactionTime,bt.Notes,bt.Reference,bt.IsBalanceUpdated,bt.ClosingPeriodId, SourceID, bt.Settings, bt.IsDeleted, bt.IsSubtractedFromBalance, bt.PaymentToInvoiceID
		FROM	[VR_AccountBalance].BillingTransaction bt  with(nolock)
		left Join [VR_AccountBalance].LiveBalance vrlb
	    on vrlb.AccountTypeID = bt.AccountTypeID and 
		   vrlb.AccountID = bt.AccountID 
        WHERE	
				 ISNULL(bt.IsDeleted, 0) = 0
				 and (@AccountsIds  is null or bt.AccountID in (select AccountID from @AccountsIdsTable))
				 and  (@TransactionTypeIds  is null or bt.TransactionTypeID in (select TransactionTypeID from @TransactionTypeIdsTable))
				 and (bt.TransactionTime >= @FromTime)
				 and (@ToTime is null or bt.TransactionTime <= @ToTime)
				 and bt.AccountTypeID = @AccountTypeID
				 and (vrlb.AccountID IS NULL OR (
				(@Status IS NULL OR vrlb.[Status] = @Status) AND
	           (@EffectiveDate IS NULL OR ((vrlb.BED <= @EffectiveDate OR vrlb.BED IS NULL) AND (vrlb.EED > @EffectiveDate OR vrlb.EED IS NULL))) AND
	          (@IsEffectiveInFuture IS NUll OR (@IsEffectiveInFuture = 1 and (vrlb.EED IS NULL or vrlb.EED >=  GETDATE())) 
		    OR  (@IsEffectiveInFuture = 0 and  vrlb.EED <=  GETDATE()))
			))
		END
	SET NOCOUNT OFF
END