CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetAllForActiveSuppliers]
	@EffectiveTime DateTime,
	@IsFuture bit,
	@ActiveSuppliersInfo TOneWhS_BE.RoutingSupplierInfo READONLY
	
AS
BEGIN

	SET NOCOUNT ON;

SELECT  sr.[ID],sr.NormalRate,sr.OtherRates,sr.PriceListID,sr.RateTypeID,sr.ZoneID,sr.BED,sr.EED
FROM	[TOneWhS_BE].SupplierRate sr WITH(NOLOCK)   
		LEFT JOIN [TOneWhS_BE].SupplierZone sz WITH(NOLOCK) ON sr.ZoneID = sz.ID 
		LEFT JOIN @ActiveSuppliersInfo s on s.SupplierId = sz.SupplierId
Where	(@IsFuture = 0 AND sr.BED <= @EffectiveTime AND (sr.EED > @EffectiveTime OR sr.EED IS NULL))
		OR
		(@IsFuture = 1 AND (sr.BED > GETDATE() OR sr.EED IS NULL))
END