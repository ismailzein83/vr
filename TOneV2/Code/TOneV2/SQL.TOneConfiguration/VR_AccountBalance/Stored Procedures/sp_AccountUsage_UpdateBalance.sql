CREATE PROCEDURE [VR_AccountBalance].[sp_AccountUsage_UpdateBalance]
	@BalanceTable [VR_AccountBalance].[BalanceTableType] READONLY,
	@CorrectionProcessID uniqueidentifier = null
AS
BEGIN

	UPDATE au
	SET 
	UsageBalance += bt.UpdateValue,
	ShouldRecreateTransaction = 1,
	CorrectionProcessID = @CorrectionProcessID
	FROM [VR_AccountBalance].AccountUsage au
	inner join @BalanceTable as bt ON  au.ID = bt.ID
END