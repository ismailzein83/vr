
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_AccountUsageOverride_Insert]
	@AccountUsageOverrideTable VR_AccountBalance.AccountUsageOverrideTable readonly
AS
BEGIN
	insert into [VR_AccountBalance].[AccountUsageOverride]([AccountTypeID], [TransactionTypeID], [AccountID], [PeriodStart], [PeriodEnd], [OverriddenByTransactionID])
	select [AccountTypeID], [TransactionTypeID], [AccountID], [PeriodStart], [PeriodEnd], [OverridenByTransactionID]
	from @AccountUsageOverrideTable
END