
Create PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetAll]
	@EffectiveTime DateTime,
	@IsFuture bit
	
AS
BEGIN

	SET NOCOUNT ON;

	SELECT  sr.[ID]
		  ,sr.NormalRate
		  ,sr.OtherRates
		  ,sr.PriceListID
		  ,sr.ZoneID
		  ,sr.BED
		  ,sr.EED
	  FROM [TOneWhS_BE].SupplierRate sr LEFT JOIN [TOneWhS_BE].SupplierZone sz ON sr.ZoneID=sz.ID 
	  Where (@IsFuture = 0 AND sr.BED <= @EffectiveTime AND (sr.EED > @EffectiveTime OR sr.EED IS NULL))
			OR
			(@IsFuture = 1 AND (sr.BED > GETDATE() OR sr.EED IS NULL))

END