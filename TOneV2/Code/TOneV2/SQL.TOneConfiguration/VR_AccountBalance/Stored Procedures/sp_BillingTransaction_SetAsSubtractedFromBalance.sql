
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_BillingTransaction_SetAsSubtractedFromBalance]
	@BillingTransactionIds nvarchar(max)
AS
BEGIN
	declare @BillingTransactionIdsTable table (BillingTransactionId bigint)
	insert into @BillingTransactionIdsTable select convert(bigint, ParsedString) from [VR_AccountBalance].[ParseStringList](@BillingTransactionIds)
	
	update [VR_AccountBalance].[BillingTransaction]
	set IsSubtractedFromBalance = 1
	where ID in (select BillingTransactionId from @BillingTransactionIdsTable)
END