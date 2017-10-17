-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[SP_SalePricelistRateChangesNew_GetAffectedCustomerIds]
	@ProcessInstanceId bigint
AS
BEGIN
	select distinct sp.OwnerID
	from TOneWhS_BE.SalePricelistRateChange_New spr WITH(NOLOCK) 
	inner join TOneWhS_BE.SalePriceList_New sp WITH(NOLOCK) on sp.ID = spr.PricelistId
	where spr.ProcessInstanceID = @ProcessInstanceID
	
END