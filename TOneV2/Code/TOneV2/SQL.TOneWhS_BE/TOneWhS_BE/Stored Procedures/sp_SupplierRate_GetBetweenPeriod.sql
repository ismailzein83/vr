

CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetBetweenPeriod]
	@From DateTime,
	@Till DateTime
AS
BEGIN
	SET NOCOUNT ON;
	declare  @From_local Datetime = @From
	declare  @Till_local Datetime = @Till

	SELECT  rate.[ID], rate.[PriceListID], rate.[ZoneID], rate.[Rate],rate.RateTypeID, rate.[BED], rate.[EED],rate.CurrencyID,rate.change
	FROM	[TOneWhS_BE].SupplierRate rate WITH(NOLOCK)
	where   (rate.BED <=@Till_local and (rate.EED is null or rate.EED > @From_local ))
END