

CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_UpdateBalanceRecreateAlertAfter]
	@LiveBalanceRecreateAlertAfterUpdateTable [VR_AccountBalance].[LiveBalanceRecreateAlertAfterUpdateTable] READONLY
AS
BEGIN

	UPDATE [VR_AccountBalance].LiveBalance
	SET 
	RecreateAlertAfter = lbtt.RecreateAlertAfter
	FROM [VR_AccountBalance].LiveBalance  lb
	inner join @LiveBalanceRecreateAlertAfterUpdateTable as lbtt ON lb.AccountTypeID = lbtt.AccountTypeId and lb.AccountID = lbtt.AccountID
	
END