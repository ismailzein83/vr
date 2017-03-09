-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_SaleEntityRoutingProduct_GetAllDefaultRPsBySellingProductsAndCustomer
	@SellingProductIds nvarchar(max),
	@CustomerId int
AS
BEGIN
	declare @SellingProductIdsTable as table (SellingProductId bigint)
	if (@SellingProductIds is not null) begin insert into @SellingProductIdsTable select convert(int, ParsedString) from TOneWhS_BE.ParseStringList(@SellingProductIds) end
	
	select [ID], [OwnerType], [OwnerID], [RoutingProductID], [BED], [EED]
	from TOneWhS_BE.SaleEntityRoutingProduct
	where ZoneID is null and ((OwnerType = 1 and OwnerID = @CustomerId) or (OwnerType = 0 and OwnerID in (select SellingProductId from @SellingProductIdsTable)))
END