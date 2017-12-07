-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_SaleZoneRoutingProduct_GetPreviews]
	@ProcessInstanceID_IN bigint
AS
BEGIN
    DECLARE @ProcessInstanceId INT
	SELECT @ProcessInstanceId  = @ProcessInstanceId_IN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	


	select ZoneName, CurrentSaleZoneRoutingProductName, IsCurrentSaleZoneRoutingProductInherited, NewSaleZoneRoutingProductName, EffectiveOn,ZoneId,NewSaleZoneRoutingProductId,CurrentSaleZoneRoutingProductId
	from TOneWhS_Sales.RP_SaleZoneRoutingProduct_Preview WITH(NOLOCK) 
	where ProcessInstanceID = @ProcessInstanceID
		
	SET NOCOUNT OFF
END