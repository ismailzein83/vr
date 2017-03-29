CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetEffectiveAfterByOwner]
	@OwnerType int,
	@OwnerID int,
	@MinDate datetime
AS
BEGIN
	select sr.ID, sr.PriceListID, sr.ZoneID, sr.CurrencyID, sr.RateTypeID, sr.Rate, sr.BED, sr.EED, sr.Change
	from [TOneWhS_BE].SaleRate sr with(nolock) inner join [TOneWhS_BE].SalePriceList sp with(nolock) on sr.PriceListID = sp.ID
	where sp.OwnerType = @OwnerType and sp.OwnerID = @OwnerID and (sr.EED is null or (sr.EED > sr.BED and sr.EED > @MinDate))
END