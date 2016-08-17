CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_UpdateAlertRule]
	@LiveBalanceAlertRuleTable [VR_AccountBalance].[LiveBalanceAlertRuleTableType] READONLY
AS
BEGIN

	UPDATE [VR_AccountBalance].LiveBalance
	SET 
	[VR_AccountBalance].LiveBalance.AlertRuleID = lbtt.AlertRuleId
	FROM [VR_AccountBalance].LiveBalance 
	inner join @LiveBalanceAlertRuleTable as lbtt ON  LiveBalance.AccountID = lbtt.AccountID

END