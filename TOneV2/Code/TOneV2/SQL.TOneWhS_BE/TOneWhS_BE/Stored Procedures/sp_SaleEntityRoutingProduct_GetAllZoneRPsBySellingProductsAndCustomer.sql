-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_SaleEntityRoutingProduct_GetAllZoneRPsBySellingProductsAndCustomer
	@SellingProductIds nvarchar(max),
	@CustomerId int,
	@SaleZoneIds nvarchar(max)
AS
BEGIN
	declare @SellingProductIdsTable as table (SellingProductId bigint)
	if (@SellingProductIds is not null) begin insert into @SellingProductIdsTable select convert(int, ParsedString) from TOneWhS_BE.ParseStringList(@SellingProductIds) end

	declare @SaleZoneIdsTable as table (SaleZoneId bigint)
	if (@SaleZoneIds is not null) begin insert into @SaleZoneIdsTable select convert(int, ParsedString) from TOneWhS_BE.ParseStringList(@SaleZoneIds) end

	select [ID], [OwnerType], [OwnerID], [RoutingProductID], [ZoneID], [BED], [EED]
	from TOneWhS_BE.SaleEntityRoutingProduct
	where ZoneID is not null
		and ZoneID in (select SaleZoneId from @SaleZoneIdsTable)
		and ((OwnerType = 1 and OwnerID = @CustomerId) or (OwnerType = 0 and OwnerID in (select SellingProductId from @SellingProductIdsTable)))
END