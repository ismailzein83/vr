

CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetBetweenPeriod]
	@From DateTime,
	@Till DateTime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT  sr.[ID]
		  ,sr.RateTypeID
		  ,sr.Rate
		  ,sr.OtherRates
		  ,sr.PriceListID
		  ,sr.ZoneID
		  ,sr.BED
		  ,sr.EED
		  ,sr.change
		  ,sr.CurrencyID
	  FROM [TOneWhS_BE].SaleRate sr 
	  Where (sr.EED is null and sr.BED<@Till) or(sr.EED>@From and sr.EED<@Till)
END