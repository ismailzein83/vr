
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetByEffective]
	@FromDate_FromOut DateTime,
	@ToDate_FromOut DateTime
AS
BEGIN
	DECLARE @FromDate DateTime
	DECLARE @ToDate DateTime

	SELECT @FromDate = @FromDate_FromOut
	SELECT @ToDate = @ToDate_FromOut

	SELECT  sr.[ID],sr.Rate,sr.PriceListID,sr.RateTypeID,sr.CurrencyID,sr.ZoneID,sr.BED,sr.EED,sr.Change
	FROM	[TOneWhS_BE].SupplierRate sr WITH(NOLOCK) 
	Where	sr.BED <=  @ToDate
			AND (sr.EED is null or sr.EED > @FromDate)
END