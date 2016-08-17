-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_AlertRuleActionExecution_GetExpiredActions]
AS
BEGIN
	SELECT	arae.ID,arae.AccountID,arae.Threshold, arae.ActionExecutionInfo, arae.ExecutionTime
	FROM	VR_AccountBalance.AlertRuleActionExecution arae with(nolock)
			left Join VR_AccountBalance.LiveBalance lb  with(nolock) on lb.AccountID = arae.AccountID
	WHERE	lb.[CurrentBalance] > arae.Threshold
END