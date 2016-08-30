-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_SaleZoneRoutingProduct_GetPreviews]
	@ProcessInstanceID bigint
AS
BEGIN
	select ZoneName, CurrentSaleZoneRoutingProductName, IsCurrentSaleZoneRoutingProductInherited, NewSaleZoneRoutingProductName, EffectiveOn
	from TOneWhS_Sales.RP_SaleZoneRoutingProduct_Preview WITH(NOLOCK) 
	where ProcessInstanceID = @ProcessInstanceID
END