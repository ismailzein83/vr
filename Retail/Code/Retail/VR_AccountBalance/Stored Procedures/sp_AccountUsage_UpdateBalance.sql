CREATE PROCEDURE [VR_AccountBalance].[sp_AccountUsage_UpdateBalance]
	@BalanceTable [VR_AccountBalance].[BalanceTableType] READONLY
AS
BEGIN

	UPDATE au
	SET 
	UsageBalance += bt.UpdateValue,
	ShouldRecreateTransaction = 1
	FROM [VR_AccountBalance].AccountUsage au
	inner join @BalanceTable as bt ON  au.ID = bt.ID
END