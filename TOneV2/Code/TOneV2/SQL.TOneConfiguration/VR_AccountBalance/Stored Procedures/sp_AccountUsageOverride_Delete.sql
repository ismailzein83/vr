-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE VR_AccountBalance.sp_AccountUsageOverride_Delete
	@DeletedTransactionIds nvarchar(max) = null
AS
BEGIN
	declare @DeletedTransactionIdsTable table (TransactionId bigint)
	if @DeletedTransactionIds is not null
	begin
		insert into @DeletedTransactionIdsTable(TransactionId)
		select ParsedString from [VR_AccountBalance].[ParseStringList](@DeletedTransactionIds)
	end

	delete from VR_AccountBalance.AccountUsageOverride where OverriddenByTransactionID in (select TransactionId from @DeletedTransactionIdsTable)
END