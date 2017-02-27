﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_GetAll]
AS
BEGIN
	SELECT	 AccountID
			,[AccountTypeID] 
			,[CurrencyID]
			,[InitialBalance]
			,[CurrentBalance]
			,[NextAlertThreshold]
			,[AlertRuleID]
			,LastExecutedActionThreshold
			,ActiveAlertsInfo
	FROM	VR_AccountBalance.LiveBalance  with(nolock)
END