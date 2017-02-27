


create PROCEDURE [VR_AccountBalance].[sp_LiveBalance_UpdateBalanceLastThreshold]
	@LiveBalanceLastThresholdUpdateTable [VR_AccountBalance].[LiveBalanceLastThresholdUpdateTable] READONLY
AS
BEGIN

	UPDATE [VR_AccountBalance].LiveBalance
	SET 
	[LastExecutedActionThreshold]= lbtt.LastExecutedActionThreshold,	
	ActiveAlertsInfo = lbtt.ActiveAlertsInfo
	FROM [VR_AccountBalance].LiveBalance  lb
	inner join @LiveBalanceLastThresholdUpdateTable as lbtt ON lb.AccountTypeID = lbtt.AccountTypeId and lb.AccountID = lbtt.AccountID
	
END