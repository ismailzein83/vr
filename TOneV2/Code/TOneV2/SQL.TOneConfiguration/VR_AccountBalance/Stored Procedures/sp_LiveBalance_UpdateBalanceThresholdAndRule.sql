

CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_UpdateBalanceThresholdAndRule]
	@LiveBalanceThresholdUpdateTable [VR_AccountBalance].LiveBalanceThresholdUpdateTable READONLY
AS
BEGIN

	UPDATE [VR_AccountBalance].LiveBalance
	SET 
	NextAlertThreshold = lbtt.NextAlertThreshold, 
	AlertRuleID = lbtt.AlertRuleId,
	RecreateAlertAfter = CASE WHEN lbtt.ClearRecreateAlertAfter = 1 THEN NULL ELSE RecreateAlertAfter END
	FROM [VR_AccountBalance].LiveBalance  lb
	inner join @LiveBalanceThresholdUpdateTable as lbtt ON lb.AccountTypeID = lbtt.AccountTypeId and lb.AccountID = lbtt.AccountID
	
END