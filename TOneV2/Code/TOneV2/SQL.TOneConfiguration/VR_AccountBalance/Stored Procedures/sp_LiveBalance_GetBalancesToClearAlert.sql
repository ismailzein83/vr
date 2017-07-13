-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_GetBalancesToClearAlert]
@AccountTypeId uniqueidentifier

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
			,lb.BED
			,lb.EED
			,lb.[Status]
	FROM VR_AccountBalance.LiveBalance lb WITH(NOLOCK) 
	WHERE lb.AccountTypeID = @AccountTypeId and lb.[CurrentBalance] > lb.LastExecutedActionThreshold 
	AND ISNULL(IsDeleted,0) = 0
END