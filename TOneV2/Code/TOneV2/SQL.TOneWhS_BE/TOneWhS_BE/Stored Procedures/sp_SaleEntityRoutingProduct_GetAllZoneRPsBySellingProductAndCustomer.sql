-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_SaleEntityRoutingProduct_GetAllZoneRPsBySellingProductAndCustomer
	@SellingProductId int,
	@CustomerId int,
	@ZoneIds nvarchar(max)
AS
BEGIN
	declare @ZoneIdsTable as table (ZoneId bigint)
	if (@ZoneIds is not null) begin insert into @ZoneIdsTable select convert(int, ParsedString) from TOneWhS_BE.ParseStringList(@ZoneIds) end

	select [ID], [OwnerType], [OwnerID], [RoutingProductID], [ZoneID], [BED], [EED]
	from TOneWhS_BE.SaleEntityRoutingProduct with(nolock)
	where ZoneID is not null and ZoneID in (select ZoneId from @ZoneIdsTable) and ((OwnerType = 0 and OwnerID = @SellingProductId) or (OwnerType = 1 and OwnerID = @CustomerId))
END