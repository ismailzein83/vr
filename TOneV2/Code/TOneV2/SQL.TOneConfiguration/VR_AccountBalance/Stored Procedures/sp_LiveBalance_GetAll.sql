-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_GetAll]
@AccountTypeId uniqueidentifier

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
			,BED
			,EED
			,[Status]
	FROM	VR_AccountBalance.LiveBalance  with(nolock)
	where	[AccountTypeID] = @AccountTypeId AND ISNULL(IsDeleted,0) = 0
END