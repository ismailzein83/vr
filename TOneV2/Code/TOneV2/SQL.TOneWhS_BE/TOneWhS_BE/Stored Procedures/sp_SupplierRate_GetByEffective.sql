
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetByEffective]
	@FromDate DateTime,
	@ToDate DateTime
AS
BEGIN
	DECLARE @FromDate_local DateTime = @FromDate
	DECLARE @ToDate_local DateTime = @ToDate

	SELECT  sr.[ID],sr.Rate,sr.PriceListID,sr.RateTypeID,sr.CurrencyID,sr.ZoneID,sr.BED,sr.EED,sr.Change
	FROM	[TOneWhS_BE].SupplierRate sr WITH(NOLOCK) 
	Where	sr.BED <=  @ToDate_local
			AND (sr.EED is null or sr.EED > @FromDate_local)
END