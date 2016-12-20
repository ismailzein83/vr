CREATE PROCEDURE [VR_AccountBalance].[sp_LiveBalance_UpdateBalance]
	
	@BalanceTable [VR_AccountBalance].[BalanceTableType] READONLY
AS
BEGIN

	UPDATE [VR_AccountBalance].LiveBalance
	SET 
	[VR_AccountBalance].LiveBalance.CurrentBalance += bt.UpdateValue
	FROM [VR_AccountBalance].LiveBalance 
	inner join @BalanceTable as bt ON  LiveBalance.ID = bt.ID

END