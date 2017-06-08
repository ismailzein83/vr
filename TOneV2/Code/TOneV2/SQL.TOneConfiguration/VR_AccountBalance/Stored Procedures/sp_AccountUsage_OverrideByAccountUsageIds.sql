-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE VR_AccountBalance.sp_AccountUsage_OverrideByAccountUsageIds
	@AccountUsageIdsToOverride nvarchar(max) = null
AS
BEGIN
	declare @AccountUsageIdTable table (AccountUsageId bigint)
	if @AccountUsageIdsToOverride is not null
	begin
		insert into @AccountUsageIdTable(AccountUsageId)
		select ParsedString from [VR_AccountBalance].[ParseStringList](@AccountUsageIdsToOverride)
	end

	update [VR_AccountBalance].[AccountUsage]
	set IsOverridden = 1, OverriddenAmount = UsageBalance
	where ID in (select AccountUsageId from @AccountUsageIdTable)
END