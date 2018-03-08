-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetAllByOwnerType]
	@OwnerType int,
	@OwnerIds nvarchar(max),
	@ZoneIds nvarchar(max),
	@GetNormalRates bit,
	@GetOtherRates bit
AS
BEGIN
	declare @OwnerIdsTable as table (OwnerId int)
	if (@OwnerIds is not null) begin insert into @OwnerIdsTable select convert(int, ParsedString) from TOneWhS_BE.ParseStringList(@OwnerIds) end

	declare @ZoneIdsTable as table (ZoneId bigint)
	if (@ZoneIds is not null) begin insert into @ZoneIdsTable select convert(bigint, ParsedString) from TOneWhS_BE.ParseStringList(@ZoneIds) end

	select rate.[ID], rate.[PriceListID], rate.[ZoneID], rate.[CurrencyID], rate.[RateTypeID], rate.[Rate], rate.[BED], rate.[EED], rate.[SourceID], rate.[Change]
	from TOneWhS_BE.SaleRate rate inner join TOneWhS_BE.SalePriceList priceList with(nolock) on rate.PriceListID = priceList.ID
	where (rate.EED is null or rate.EED > rate.BED)
		and ((rate.RateTypeID is null and @GetNormalRates = 1) or (rate.RateTypeID is not null and @GetOtherRates = 1))
		and priceList.OwnerType = @OwnerType
		and priceList.OwnerID in (select OwnerId from @OwnerIdsTable)
		and (@ZoneIds is null or rate.ZoneID in (select ZoneId from @ZoneIdsTable))
END