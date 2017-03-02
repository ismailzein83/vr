-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_SaleRate_GetAllByOwner
	@OwnerType int,
	@OwnerId int,
	@SaleZoneIds nvarchar(max) = null
AS
BEGIN
	declare @SaleZoneIdsTable as table (SaleZoneId bigint)
	if (@SaleZoneIds is not null) begin insert into @SaleZoneIdsTable select convert(int, ParsedString) from TOneWhS_BE.ParseStringList(@SaleZoneIds) end
	
	select rate.[ID], rate.[PriceListID], rate.[ZoneID], rate.[CurrencyID], rate.[RateTypeID], rate.[Rate], rate.[BED], rate.[EED], rate.[SourceID], rate.[Change]
	from TOneWhS_BE.SaleRate rate inner join TOneWhS_BE.SalePriceList pricelist on rate.PriceListID = pricelist.ID
	where (rate.EED is null or rate.EED > rate.BED) and pricelist.OwnerType = @OwnerType and pricelist.OwnerID = @OwnerId and (@SaleZoneIds is null or rate.ZoneID in (select SaleZoneId from @SaleZoneIdsTable))
END