CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetAll]
	@EffectiveTime DateTime,
	@IsFuture bit
	
AS
BEGIN

	SET NOCOUNT ON;

SELECT  sr.[ID],sr.Rate,sr.PriceListID,sr.RateTypeID,sr.ZoneID,sr.BED,sr.EED,sr.Change
FROM	[TOneWhS_BE].SupplierRate sr WITH(NOLOCK) 
		LEFT JOIN [TOneWhS_BE].SupplierZone sz WITH(NOLOCK) ON sr.ZoneID=sz.ID 
Where	(@IsFuture = 0 AND sr.BED <= @EffectiveTime AND (sr.EED > @EffectiveTime OR sr.EED IS NULL))
		OR
		(@IsFuture = 1 AND (sr.BED > GETDATE() OR sr.EED IS NULL))

END