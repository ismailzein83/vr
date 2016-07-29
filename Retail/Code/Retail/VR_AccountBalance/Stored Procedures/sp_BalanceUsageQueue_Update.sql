create PROCEDURE [VR_AccountBalance].[sp_BalanceUsageQueue_Update]
	@UsageDetails varbinary(max)
AS
BEGIN
		insert into [VR_AccountBalance].BalanceUsageQueue
		(UsageDetails) values (@UsageDetails)
END