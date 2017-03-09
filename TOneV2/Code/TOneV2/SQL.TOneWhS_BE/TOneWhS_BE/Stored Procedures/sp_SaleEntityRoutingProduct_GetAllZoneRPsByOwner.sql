-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_SaleEntityRoutingProduct_GetAllZoneRPsByOwner
	@OwnerType tinyint,
	@OwnerId int,
	@SaleZoneIds nvarchar(max)
AS
BEGIN
	declare @SaleZoneIdsTable as table (SaleZoneId bigint)
	if (@SaleZoneIds is not null) begin insert into @SaleZoneIdsTable select convert(int, ParsedString) from TOneWhS_BE.ParseStringList(@SaleZoneIds) end

	select [ID], [OwnerType], [OwnerID], [ZoneID], [RoutingProductID], [BED], [EED]
	from TOneWhS_BE.SaleEntityRoutingProduct
	where ZoneID is not null and ZoneID in (select SaleZoneId from @SaleZoneIdsTable) and OwnerType = @OwnerType and OwnerID = @OwnerId
END