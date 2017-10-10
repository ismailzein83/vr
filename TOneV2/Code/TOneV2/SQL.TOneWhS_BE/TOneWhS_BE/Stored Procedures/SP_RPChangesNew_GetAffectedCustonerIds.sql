-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[SP_RPChangesNew_GetAffectedCustonerIds]
	@ProcessInstanceId bigint
AS
BEGIN
	Select Distinct CustomerId
	from [TOneV2_Dev].[TOneWhS_BE].[SalePricelistRPChange_New]
	where ProcessInstanceID = @ProcessInstanceId
END