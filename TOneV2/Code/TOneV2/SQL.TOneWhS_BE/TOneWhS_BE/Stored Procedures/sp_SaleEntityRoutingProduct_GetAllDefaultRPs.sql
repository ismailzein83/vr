CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetAllDefaultRPs]
AS
BEGIN
	select [ID], [OwnerType], [OwnerID], [RoutingProductID], [BED], [EED]
	from TOneWhS_BE.SaleEntityRoutingProduct WITH (NOLOCK)
	where ZoneID is null and (EED is null or BED <> EED)
END