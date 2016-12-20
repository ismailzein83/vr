CREATE PROCEDURE [VR_AccountBalance].[sp_AccountUsage_UpdateBalance]
	@BalanceTable [VR_AccountBalance].[BalanceTableType] READONLY
AS
BEGIN

	UPDATE [VR_AccountBalance].AccountUsage
	SET 
	[VR_AccountBalance].AccountUsage.UsageBalance += bt.UpdateValue
	FROM [VR_AccountBalance].AccountUsage 
	inner join @BalanceTable as bt ON  AccountUsage.ID = bt.ID

END