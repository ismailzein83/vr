
CREATE PROCEDURE [VR_AccountBalance].sp_LiveBalance_UpdateBalanceThresholdAndRule
	@LiveBalanceThresholdUpdateTable [VR_AccountBalance].LiveBalanceThresholdUpdateTable READONLY
AS
BEGIN

	UPDATE [VR_AccountBalance].LiveBalance
	SET 
	[VR_AccountBalance].LiveBalance.NextAlertThreshold = lbtt.NextAlertThreshold, 
	[VR_AccountBalance].LiveBalance.AlertRuleID = lbtt.AlertRuleId
	FROM [VR_AccountBalance].LiveBalance  lb
	inner join @LiveBalanceThresholdUpdateTable as lbtt ON lb.AccountTypeID = lbtt.AccountTypeId and lb.AccountID = lbtt.AccountID
	
END