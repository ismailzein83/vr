

CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetBetweenPeriod]
	@From DateTime,
	@Till DateTime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT  sr.[ID]
		  ,sr.Rate
		  ,sr.OtherRates
		  ,sr.PriceListID
		  ,sr.ZoneID
		  ,sr.BED
		  ,sr.EED
		  ,sr.change
	  FROM [TOneWhS_BE].SaleRate sr 
	  Where sr.BED < @From AND (sr.EED IS NULL OR sr.EED > @Till)
END