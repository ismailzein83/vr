CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetAll]
	@EffectiveTime DateTime,
	@IsFuture bit
	
AS
BEGIN

	Declare @EffectiveTime_local DateTime = @EffectiveTime
	Declare @IsFuture_local bit = @IsFuture

	SELECT  sr.[ID],sr.Rate,sr.PriceListID,sr.RateTypeID,sr.ZoneID,sr.BED,sr.EED,sr.Change
	FROM	[TOneWhS_BE].SupplierRate sr WITH(NOLOCK) 
			LEFT JOIN [TOneWhS_BE].SupplierZone sz WITH(NOLOCK) ON sr.ZoneID=sz.ID 
	Where	(@IsFuture_local = 0 AND sr.BED <= @EffectiveTime_local AND (sr.EED > @EffectiveTime_local OR sr.EED IS NULL))
			OR
			(@IsFuture_local = 1 AND (sr.BED > GETDATE() OR sr.EED IS NULL))
END