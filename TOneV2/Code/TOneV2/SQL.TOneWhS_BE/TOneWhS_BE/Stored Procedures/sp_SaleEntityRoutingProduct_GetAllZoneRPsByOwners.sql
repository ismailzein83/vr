-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetAllZoneRPsByOwners]
	@SellingProductIds nvarchar(max),
	@CustomerIds nvarchar(max),
	@ZoneIds nvarchar(max)
AS
BEGIN
	declare @SellingProductIdsTable as table (SellingProductId int);
	if (@SellingProductIds is not null) begin insert into @SellingProductIdsTable select convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@SellingProductIds) end;
	
	declare @CustomerIdsTable as table (CustomerId int);
	if (@CustomerIds is not null) begin insert into @CustomerIdsTable select convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@CustomerIds) end;
	
	declare @ZoneIdsTable as table (ZoneId bigint);
	if (@ZoneIds is not null) begin insert into @ZoneIdsTable select convert(bigint, ParsedString) from [TOneWhS_BE].[ParseStringList](@ZoneIds) end;

	select ID, OwnerType, OwnerID, ZoneID, RoutingProductID, BED, EED
	from TOneWhS_BE.SaleEntityRoutingProduct with(nolock)
	where (EED is null or EED > BED)
		and (ZoneID is not null and ZoneID in (select ZoneId from @ZoneIdsTable))
		and ((OwnerType = 0 and OwnerID in (select SellingProductId from @SellingProductIdsTable)) or (OwnerType = 1 and OwnerID in (select CustomerId from @CustomerIdsTable)))
END