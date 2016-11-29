CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_UpdateBalanceThreshold]
	@AccountTypeId uniqueidentifier,
	@LiveBalanceThresholdTable [VR_AccountBalance].[LiveBalanceThresholdTableType] READONLY
AS
BEGIN

	UPDATE [VR_AccountBalance].LiveBalance
	SET 
	[VR_AccountBalance].LiveBalance.NextAlertThreshold = lbtt.Threshold, 
	[VR_AccountBalance].LiveBalance.AlertRuleID = lbtt.AlertRuleId,
	[VR_AccountBalance].LiveBalance.ThresholdActionIndex = lbtt.ThresholdActionIndex

	FROM [VR_AccountBalance].LiveBalance 
	inner join @LiveBalanceThresholdTable as lbtt ON  LiveBalance.AccountID = lbtt.AccountID
	where AccountTypeID = @AccountTypeId

END