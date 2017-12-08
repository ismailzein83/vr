﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_SaleRate_GetAllByOwners
	@SellingProductIds nvarchar(max),
	@CustomerIds nvarchar(max),
	@ZoneIds nvarchar(max)
AS
BEGIN
	declare @SellingProductIdsTable as table (SellingProductId int)
	if (@SellingProductIds is not null) begin insert into @SellingProductIdsTable select convert(int, ParsedString) from TOneWhS_BE.ParseStringList(@SellingProductIds) end

	declare @CustomerIdsTable as table (CustomerId int)
	if (@CustomerIds is not null) begin insert into @CustomerIdsTable select convert(int, ParsedString) from TOneWhS_BE.ParseStringList(@CustomerIds) end

	declare @ZoneIdsTable as table (ZoneId bigint)
	if (@ZoneIds is not null) begin insert into @ZoneIdsTable select convert(bigint, ParsedString) from TOneWhS_BE.ParseStringList(@ZoneIds) end

	select rate.[ID], rate.[PriceListID], rate.[ZoneID], rate.[CurrencyID], rate.[RateTypeID], rate.[Rate], rate.[BED], rate.[EED], rate.[SourceID], rate.[Change]
	from TOneWhS_BE.SaleRate rate inner join TOneWhS_BE.SalePriceList priceList with(nolock) on rate.PriceListID = priceList.ID
	where (rate.EED is null or rate.EED > rate.BED)
		and ((OwnerType = 0 and OwnerID in (select SellingProductId from @SellingProductIdsTable)) or (OwnerType = 1 and OwnerID in (select CustomerId from @CustomerIdsTable)))
		and rate.ZoneID in (select ZoneId from @ZoneIdsTable)
END