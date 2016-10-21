CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetFutureByOwner]
	@OwnerType int,
	@OwnerId int
AS
BEGIN
	declare @Now datetime;
	set @Now = getdate();

    select	[ID], [PriceListID], [ZoneID], [CurrencyID], [RateTypeID], [Rate], [BED], [EED], [timestamp], [SourceID], [Change]
	from	[TOneWhS_BE].[SaleRate] WITH(NOLOCK) 
	where	PriceListId in (select Id from TOneWhS_BE.SalePriceList WITH(NOLOCK) where OwnerType = @OwnerType and OwnerId = @OwnerId) and (EED is null or BED > @Now)
END