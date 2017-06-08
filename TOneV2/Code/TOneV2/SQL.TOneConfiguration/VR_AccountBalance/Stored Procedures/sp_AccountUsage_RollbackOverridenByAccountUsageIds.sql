-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE VR_AccountBalance.sp_AccountUsage_RollbackOverridenByAccountUsageIds
	@OverridenAccountUsageIdsToRollback nvarchar(max) = null
AS
BEGIN
	declare @OverridenAccountUsageIdsToRollbackTable table (AccountUsageId bigint)
	if @OverridenAccountUsageIdsToRollback is not null
	begin
		insert into @OverridenAccountUsageIdsToRollbackTable(AccountUsageId)
		select ParsedString from [VR_AccountBalance].[ParseStringList](@OverridenAccountUsageIdsToRollback)
	end

	update [VR_AccountBalance].[AccountUsage]
	set IsOverridden = null, OverriddenAmount = null
	where ID in (select AccountUsageId from @OverridenAccountUsageIdsToRollbackTable)
END