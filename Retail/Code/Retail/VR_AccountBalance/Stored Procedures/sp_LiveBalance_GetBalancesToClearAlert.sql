-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_GetBalancesToClearAlert]
AS
BEGIN
	
	SELECT	 lb.CurrentBalance
			,lb.AccountId
			,lb.AccountTypeID
			,lb.UsageBalance
			,lb.CurrencyID
			,lb.AlertRuleID
			,lb.InitialBalance
			,lb.NextAlertThreshold
			,lb.LastExecutedActionThreshold
			,lb.ThresholdActionIndex
			,lb.ActiveAlertsInfo
	FROM VR_AccountBalance.LiveBalance lb WITH(NOLOCK) 
	WHERE lb.[CurrentBalance] > lb.LastExecutedActionThreshold 

END