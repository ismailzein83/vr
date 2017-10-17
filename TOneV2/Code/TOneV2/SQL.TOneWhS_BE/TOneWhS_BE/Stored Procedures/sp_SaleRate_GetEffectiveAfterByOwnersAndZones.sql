-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetEffectiveAfterByOwnersAndZones]
	@OwnerType int,
	@OwnerIds nvarchar(max),
	@ZoneIds nvarchar(max),
	@MinimumDate datetime
AS
BEGIN
	declare @OwnerIdTable table (OwnerId int)
	insert into @OwnerIdTable (OwnerId) select convert(int, ParsedString) from TOneWhS_BE.ParseStringList(@OwnerIds)

	declare @ZoneIdTable table (ZoneId bigint)
	insert into @ZoneIdTable (ZoneId) select convert(bigint, ParsedString) from TOneWhS_BE.ParseStringList(@ZoneIds)

	select	rate.[ID], rate.[PriceListID], rate.[ZoneID], rate.[CurrencyID], rate.[RateTypeID], rate.[Rate], rate.[BED], rate.[EED], rate.[Change]
	from	TOneWhS_BE.SaleRate rate WITH(NOLOCK) inner join TOneWhS_BE.SalePriceList pricelist with(nolock) on rate.PriceListID = pricelist.ID
	where (rate.EED is null or rate.EED > @MinimumDate)
		and pricelist.OwnerType = @OwnerType
		and pricelist.OwnerID in (select OwnerId from @OwnerIdTable)
		and rate.ZoneID in (select ZoneId from @ZoneIdTable)
END