

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_AccountUsage_GetByTransactionAccountUsageQueries]
	@TransactionAccountUsageQueryTable VR_AccountBalance.TransactionAccountUsageQueryTable readonly
AS
BEGIN
	select usageTable.[ID],
		usageTable.[AccountTypeID],
		usageTable.[TransactionTypeID],
		usageTable.[AccountID],
		usageTable.[CurrencyId],
		usageTable.[PeriodStart],
		usageTable.[PeriodEnd],
		usageTable.[UsageBalance],
		usageTable.[IsOverridden],
		usageTable.[OverriddenAmount],
		usageTable.[CorrectionProcessID]

	from [VR_AccountBalance].[AccountUsage] usageTable inner join @TransactionAccountUsageQueryTable queryTable
		on usageTable.AccountTypeID = queryTable.AccountTypeID
			and usageTable.AccountID = queryTable.AccountID
			and usageTable.TransactionTypeID = queryTable.TransactionTypeID
			and usageTable.PeriodStart >= queryTable.PeriodStart
			and usageTable.PeriodEnd <= queryTable.PeriodEnd
END