CREATE PROCEDURE [VR_AccountBalance].[sp_BalanceUsageQueue_Update]
	@AccountTypeID uniqueidentifier,
	@QueueType int,
	@UsageDetails varbinary(max)
AS
BEGIN
		insert into [VR_AccountBalance].BalanceUsageQueue
		([AccountTypeID],QueueType, UsageDetails) values (@AccountTypeID,@QueueType, @UsageDetails)
END