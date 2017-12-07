-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetAllBySellingProductAndCustomer]
	@SaleZoneIds nvarchar(max) = null,
	@SellingProductId int,
	@CustomerId int,
	@GetNormalRates bit,
	@GetOtherRates bit
AS
BEGIN
	declare @SaleZoneIdsTable as table (SaleZoneId bigint)
	if (@SaleZoneIds is not null) begin insert into @SaleZoneIdsTable select convert(bigint, ParsedString) from TOneWhS_BE.ParseStringList(@SaleZoneIds) end

	select rate.[ID], rate.[PriceListID], rate.[ZoneID], rate.[CurrencyID], rate.[RateTypeID], rate.[Rate], rate.[BED], rate.[EED], rate.[SourceID], rate.[Change]
	from TOneWhS_BE.SaleRate rate WITH(NOLOCK) inner join TOneWhS_BE.SalePriceList pricelist WITH(NOLOCK) on rate.PriceListID = pricelist.ID
	where (rate.EED is null or rate.EED > rate.BED)
		and ((rate.RateTypeID is null and @GetNormalRates = 1) or (RateTypeID is not null and @GetOtherRates = 1))
		and rate.ZoneID in (select SaleZoneId from @SaleZoneIdsTable)
		and ((pricelist.OwnerType = 0 and pricelist.OwnerID = @SellingProductId) or (pricelist.OwnerType = 1 and pricelist.OwnerID = @CustomerId))
END