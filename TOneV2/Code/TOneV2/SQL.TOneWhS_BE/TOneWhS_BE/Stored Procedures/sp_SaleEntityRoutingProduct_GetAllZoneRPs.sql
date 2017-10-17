
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetAllZoneRPs]	
AS
BEGIN	
	select [ID], [OwnerType], [OwnerID], [ZoneID], [RoutingProductID], [BED], [EED]
	from TOneWhS_BE.SaleEntityRoutingProduct WITH (NOLOCK)
	where ZoneID is not null and (EED is null or BED <> EED)
END