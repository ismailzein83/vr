-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[SP_SalePricelistRPChangesNew_GetAffectedCustomerIds]
	@ProcessInstanceId bigint
AS
BEGIN
	Select	Distinct CustomerId
	from	[TOneWhS_BE].[SalePricelistRPChange_New] with(Nolock)
	where	ProcessInstanceID = @ProcessInstanceId
END