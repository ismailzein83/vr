CREATE PROCEDURE [VR_AccountBalance].[sp_AlertRuleThresholdAction_AddOrUpdate]
	@AlertRuleThresholdActionsTable [VR_AccountBalance].AlertRuleThresholdActionsTableType READONLY
AS
BEGIN

	delete from [VR_AccountBalance].AlertRuleThresholdAction 
	WHERE RuleID IN (Select arta.RuleID FROM [VR_AccountBalance].AlertRuleThresholdAction arta 
	inner join @AlertRuleThresholdActionsTable as artat ON  artat.RuleId = arta.RuleId)

	INSERT INTO [VR_AccountBalance].AlertRuleThresholdAction (RuleID,Threshold,ThresholdActionIndex)
	SELECT artat.RuleID, artat.Threshold ,artat.ThresholdActionIndex  FROM @AlertRuleThresholdActionsTable artat

END