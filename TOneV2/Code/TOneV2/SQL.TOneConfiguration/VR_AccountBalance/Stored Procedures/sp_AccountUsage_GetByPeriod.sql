-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_AccountUsage_GetByPeriod]
	@AccountTypeID uniqueidentifier,
	@PeriodStart Datetime,
	@TransactionTypeId uniqueidentifier
AS
BEGIN

	--SELECT  AccountID 
	--INTO #OverriddenUsages
	--FROM [VR_AccountBalance].[AccountUsageOverride] with(nolock)
	--WHERE AccountTypeID = @AccountTypeID AND TransactionTypeID = @TransactionTypeId AND [PeriodStart] <= @PeriodStart AND [PeriodEnd] >= @PeriodStart

	--SELECT usages.ID, usages.AccountID,usages.TransactionTypeID, CASE WHEN overriddenUsages.AccountID IS NOT NULL THEN 1 ELSE 0 END AS IsOverridden
	--FROM VR_AccountBalance.AccountUsage usages with(nolock)
	--LEFT JOIN #OverriddenUsages overriddenUsages ON usages.AccountID = overriddenUsages.AccountID
	--where usages.AccountTypeID = @AccountTypeID AND usages.PeriodStart = @PeriodStart AND usages.TransactionTypeId= @TransactionTypeId

	SELECT ID, AccountID,TransactionTypeID
	FROM VR_AccountBalance.AccountUsage with(nolock)
	where AccountTypeID = @AccountTypeID AND PeriodStart = @PeriodStart AND TransactionTypeId= @TransactionTypeId
END