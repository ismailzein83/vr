-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[SP_RateChangesNew_GetAffectedCustonerIds]
	@ProcessInstanceId bigint
AS
BEGIN
	select distinct sp.OwnerID
	from TOneWhS_BE.SalePricelistRateChange_New spr WITH(NOLOCK) 
	join TOneWhS_BE.SalePriceList_New sp on sp.ID = spr.PricelistId
	where spr.ProcessInstanceID = @ProcessInstanceID
	
END