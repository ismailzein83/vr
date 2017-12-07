-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_SaleEntityRoutingProduct_GetAllDefaultRPsByOwners
	@SellingProductIds nvarchar(max),
	@CustomerIds nvarchar(max)
AS
BEGIN
	begin declare @SellingProductIdsTable as table (SellingProductId int); insert into @SellingProductIdsTable select convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@SellingProductIds) end;
	begin declare @CustomerIdsTable as table (CustomerId int); insert into @CustomerIdsTable select convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@CustomerIds) end;

	select ID, OwnerType, OwnerID, ZoneID, RoutingProductID, BED, EED
	from TOneWhS_BE.SaleEntityRoutingProduct with(nolock)
	where ZoneID is null
		and (EED is null or EED > BED)
		and ((OwnerType = 0 and OwnerID in (select SellingProductId from @SellingProductIdsTable)) or (OwnerType = 1 and OwnerID in (select CustomerId from @CustomerIdsTable)))
END