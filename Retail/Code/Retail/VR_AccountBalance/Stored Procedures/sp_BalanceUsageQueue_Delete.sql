create PROCEDURE [VR_AccountBalance].sp_BalanceUsageQueue_Delete
	@BalanceUsageQueueId bigint
AS
BEGIN
	Delete From [VR_AccountBalance].BalanceUsageQueue Where ID = @BalanceUsageQueueId
END