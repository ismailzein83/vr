
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_AccountUsage_GetOverridenByDeletedTransactionIds]
	@DeletedTransactionIds nvarchar(max)
AS
BEGIN
	declare @DeletedTransactionIdsTable table (TransactionId bigint)
	insert into @DeletedTransactionIdsTable(TransactionId)
	select ParsedString from [VR_AccountBalance].[ParseStringList](@DeletedTransactionIds);

	with AccountUsageOverrides as (select AccountTypeID, AccountID, TransactionTypeID, PeriodStart, PeriodEnd, OverriddenByTransactionID from VR_AccountBalance.AccountUsageOverride
									WHERE OverriddenByTransactionID in (select TransactionId from @DeletedTransactionIdsTable))

	select usage.[ID]
		,usage.[AccountTypeID]
		,usage.[TransactionTypeID]
		,usage.[AccountID]
		,usage.[CurrencyId]
		,usage.[PeriodStart]
		,usage.[PeriodEnd]
		,usage.[UsageBalance]
		,usage.[IsOverridden]
		,usage.[OverriddenAmount]
		,usage.[CorrectionProcessID]

	from [VR_AccountBalance].[AccountUsage] usage
	inner join AccountUsageOverrides usageOverride

	on usage.AccountTypeID = usageOverride.AccountTypeID
		and usage.AccountID = usageOverride.AccountID
		and usage.TransactionTypeID = usageOverride.TransactionTypeID
		and usage.PeriodStart >= usageOverride.PeriodStart
		and usage.PeriodEnd <= usageOverride.PeriodEnd
END