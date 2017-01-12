

CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetBetweenPeriod]
	@From DateTime,
	@Till DateTime
AS
BEGIN
	SET NOCOUNT ON;
	declare  @From_local Datetime = @From
	declare  @Till_local Datetime = @Till

	SELECT  sr.[ID]
		  ,sr.RateTypeID
		  ,sr.Rate
		  ,sr.PriceListID
		  ,sr.ZoneID
		  ,sr.BED
		  ,sr.EED
		  ,sr.change
		  ,sr.CurrencyID
	  FROM [TOneWhS_BE].SaleRate sr 
	  Where (sr.BED <=@Till_local and (sr.EED is null or sr.EED > @From_local ))
END