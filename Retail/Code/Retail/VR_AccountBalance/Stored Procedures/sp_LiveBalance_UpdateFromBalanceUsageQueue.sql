CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_UpdateFromBalanceUsageQueue]
	@AccountTypeId Uniqueidentifier,
	@LiveBalanceTable [VR_AccountBalance].[LiveBalanceTableType] READONLY
AS
BEGIN

	UPDATE [VR_AccountBalance].LiveBalance
	SET 
	[VR_AccountBalance].LiveBalance.UsageBalance += lbt.UpdateValue, 
	[VR_AccountBalance].LiveBalance.CurrentBalance += lbt.UpdateValue
	FROM [VR_AccountBalance].LiveBalance 
	inner join @LiveBalanceTable as lbt ON  LiveBalance.AccountID = lbt.AccountID
	where AccountTypeID = @AccountTypeId

END