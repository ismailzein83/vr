CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetEffectiveBySuppliers]
	@EffectiveTime DATETIME = NULL,
	@IsFuture bit,
	@ActiveSuppliersInfo TOneWhS_BE.RoutingSupplierInfo READONLY
	
AS
BEGIN

	DECLARE @EffectiveTime_local DATETIME = @EffectiveTime
	DECLARE @IsFuture_local bit = @IsFuture
	DECLARE @ActiveSuppliersInfo_local TOneWhS_BE.RoutingSupplierInfo
	Insert into @ActiveSuppliersInfo_local select * from @ActiveSuppliersInfo

SELECT  sr.[ID],sr.Rate,sr.PriceListID,sr.RateTypeID,sr.CurrencyId,sr.ZoneID,sr.BED,sr.EED,sr.Change
FROM	[TOneWhS_BE].SupplierRate sr WITH(NOLOCK) 	  
		JOIN [TOneWhS_BE].SupplierPriceList pl WITH(NOLOCK) ON sr.PriceListID = pl.ID 
		JOIN @ActiveSuppliersInfo_local s on s.SupplierId = pl.SupplierId
Where	(@IsFuture_local = 0 AND sr.BED <= @EffectiveTime_local AND (sr.EED > @EffectiveTime_local OR sr.EED IS NULL))
		OR
		(@IsFuture_local = 1 AND (sr.BED > GETDATE() OR sr.EED IS NULL))

END