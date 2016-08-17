﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_GetAll]
AS
BEGIN
	SELECT	AccountID,[CurrencyID],[InitialBalance],[UsageBalance],[CurrentBalance],[CurrentAlertThreshold],[NextAlertThreshold],[AlertRuleID],ThresholdActionIndex
	FROM	VR_AccountBalance.LiveBalance  with(nolock)
END