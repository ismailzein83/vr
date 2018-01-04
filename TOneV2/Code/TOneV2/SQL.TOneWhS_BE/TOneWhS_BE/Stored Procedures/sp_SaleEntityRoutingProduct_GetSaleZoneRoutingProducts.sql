

CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetSaleZoneRoutingProducts]
	@OwnerType int,
	@OwnerId int
AS
BEGIN
	select ID, OwnerType, OwnerID, ZoneID, RoutingProductID, BED, EED
	from [TOneWhS_BE].SaleEntityRoutingProduct WITH(NOLOCK) 
	where OwnerType = @OwnerType and OwnerId = @OwnerId and ZoneID is not null 
END