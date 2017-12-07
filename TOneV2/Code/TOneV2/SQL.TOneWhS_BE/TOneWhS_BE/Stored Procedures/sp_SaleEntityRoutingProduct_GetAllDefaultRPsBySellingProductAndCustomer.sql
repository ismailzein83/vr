-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_SaleEntityRoutingProduct_GetAllDefaultRPsBySellingProductAndCustomer
	@SellingProductId int,
	@CustomerId int,
	@ZoneIds nvarchar(max)
AS
BEGIN
	declare @ZoneIdsTable as table (ZoneId bigint)
	if (@ZoneIds is not null) begin insert into @ZoneIdsTable select convert(int, ParsedString) from TOneWhS_BE.ParseStringList(@ZoneIds) end

	select [ID], [OwnerType], [OwnerID], [RoutingProductID], [BED], [EED]
	from TOneWhS_BE.SaleEntityRoutingProduct with(nolock)
	where ZoneID is null and ((OwnerType = 0 and OwnerID = @SellingProductId) or (OwnerType = 1 and OwnerID = @CustomerId))
END