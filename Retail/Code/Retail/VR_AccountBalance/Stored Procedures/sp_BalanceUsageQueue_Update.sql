CREATE PROCEDURE [VR_AccountBalance].[sp_BalanceUsageQueue_Update]
	@AccountTypeID uniqueidentifier,
	@UsageDetails varbinary(max)
AS
BEGIN
		insert into [VR_AccountBalance].BalanceUsageQueue
		([AccountTypeID], UsageDetails) values (@AccountTypeID, @UsageDetails)
END