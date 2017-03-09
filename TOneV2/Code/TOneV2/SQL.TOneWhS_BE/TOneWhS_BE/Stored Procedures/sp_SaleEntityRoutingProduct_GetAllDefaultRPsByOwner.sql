-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_SaleEntityRoutingProduct_GetAllDefaultRPsByOwner
	@OwnerType tinyint,
	@OwnerId int
AS
BEGIN
	select [ID], [OwnerType], [OwnerID], [RoutingProductID], [BED], [EED]
	from TOneWhS_BE.SaleEntityRoutingProduct
	where ZoneID is null and OwnerType = @OwnerType and OwnerID = @OwnerId
END