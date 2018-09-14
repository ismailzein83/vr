-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetSaleZoneRoutingProductsEffectiveAfter]
	@OwnerType int,
	@OwnerId int,
	@MinDate datetime
AS
BEGIN
	select ID, OwnerType, OwnerID, ZoneID, RoutingProductID, BED, EED
	from [TOneWhS_BE].SaleEntityRoutingProduct WITH(NOLOCK) 
	where OwnerType = @OwnerType and OwnerId = @OwnerId and ZoneID is not null and (EED is null or (EED > @MinDate and EED <> BED))
END