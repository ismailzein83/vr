-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_GetBalancesForAlert]
AS
BEGIN
	
	SELECT	 lb.CurrentBalance
			,lb.AccountId
			,lb.AccountTypeID
			,lb.CurrencyID
			,lb.AlertRuleID
			,lb.InitialBalance
			,lb.NextAlertThreshold
			,lb.LastExecutedActionThreshold
			,lb.ActiveAlertsInfo
	FROM VR_AccountBalance.LiveBalance lb WITH(NOLOCK) 
	WHERE lb.[CurrentBalance] <= lb.NextAlertThreshold and (lb.LastExecutedActionThreshold IS NULL OR lb.NextAlertThreshold  < lb.LastExecutedActionThreshold)

END