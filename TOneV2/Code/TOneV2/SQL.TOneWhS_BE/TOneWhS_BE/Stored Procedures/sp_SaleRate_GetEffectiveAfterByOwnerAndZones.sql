-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetEffectiveAfterByOwnerAndZones]
	@OwnerType_In int,
	@OwnerId_In int,
	@ZoneIds_In nvarchar(max),
	@EffectiveOn_In datetime
AS
BEGIN
	declare @OwnerType int; select @OwnerType = @OwnerType_In
	declare @OwnerId int; select @OwnerId = @OwnerId_In
	declare @ZoneIds nvarchar(max); select @ZoneIds = @ZoneIds_In
	declare @EffectiveOn datetime; select @EffectiveOn = @EffectiveOn_In
	declare @Today datetime; set @Today = getdate()

	declare @ZoneIdTable table (ZoneId bigint)
	insert into @ZoneIdTable (ZoneId)
	select convert(int, ParsedString) from TOneWhS_BE.ParseStringList(@ZoneIds)

	select rate.[ID], rate.[PriceListID], rate.[ZoneID], rate.[CurrencyID], rate.[RateTypeID], rate.[Rate], rate.[BED], rate.[EED], rate.[Change]
	from TOneWhS_BE.SaleRate rate inner join TOneWhS_BE.SalePriceList pricelist with(nolock) on rate.PriceListID = pricelist.ID
	where rate.BED <= @Today and rate.EED is not null and rate.EED > rate.BED -- Exclude future and deleted rates
		and pricelist.OwnerType = @OwnerType
		and pricelist.OwnerID = @OwnerId
		and rate.ZoneID in (select ZoneId from @ZoneIdTable)
		and rate.EED > @EffectiveOn
END