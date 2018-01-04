create PROCEDURE [VR_AccountBalance].[sp_AccountUsage_GetByTime]
	@AccountUsagesByTimeTable VR_AccountBalance.[AccountUsagesByTimeTable] READONLY,
	@AccountTypeId uniqueidentifier,
	@TransactionTypeIds varchar(max)
AS
BEGIN
SET NOCOUNT ON;
	DECLARE @TransactionTypeIdsTable TABLE (TransactionTypeId uniqueidentifier)
	INSERT INTO @TransactionTypeIdsTable (TransactionTypeId)
	SELECT CONVERT(uniqueidentifier, ParsedString) FROM [VR_Invoice].[ParseStringList](@TransactionTypeIds)
	
	SELECT au.ID, au.AccountTypeID,TransactionTypeID, au.AccountID,au.CurrencyId,PeriodStart,au.PeriodEnd,UsageBalance, IsOverridden, OverriddenAmount, CorrectionProcessID
	FROM	VR_AccountBalance.AccountUsage au with(nolock)
	JOIN	@AccountUsagesByTimeTable att on au.AccountID = att.AccountID AND au.PeriodEnd > att.PeriodEnd
	WHERE	isnull(au.IsOverridden, 0) = 0
			AND au.AccountTypeId = @AccountTypeId 
			AND (@TransactionTypeIds IS NULL OR au.TransactionTypeID IN (SELECT TransactionTypeId FROM @TransactionTypeIdsTable))
End