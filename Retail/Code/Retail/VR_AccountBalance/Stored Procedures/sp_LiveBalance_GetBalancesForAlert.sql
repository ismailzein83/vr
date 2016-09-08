-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_GetBalancesForAlert]
AS
BEGIN
 WITH AlertRuleActionExecution (AccountID, Threshold) AS (Select AccountID, MIN(Threshold) from [VR_AccountBalance].AlertRuleActionExecution WITH(NOLOCK)  group by  AccountID)
	
	SELECT lb.AccountID
         ,lb.[AlertRuleID]
	     , arta.ThresholdActionIndex
	     , arta.Threshold
	FROM VR_AccountBalance.LiveBalance lb WITH(NOLOCK) 
	left Join VR_AccountBalance.AlertRuleThresholdAction arta  WITH(NOLOCK) on lb.[AlertRuleID] = arta.RuleId
	left join AlertRuleActionExecution arae  WITH(NOLOCK) on arae.AccountID = lb.AccountID 
	WHERE lb.[CurrentBalance] < arta.Threshold and (arae.Threshold IS NULL OR arta.Threshold  < arae.Threshold)

END