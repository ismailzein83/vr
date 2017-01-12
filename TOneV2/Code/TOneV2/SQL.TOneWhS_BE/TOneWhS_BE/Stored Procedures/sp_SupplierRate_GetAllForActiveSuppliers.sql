CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetAllForActiveSuppliers]
	@EffectiveTime DateTime,
	@IsFuture bit,
	@ActiveSuppliersInfo TOneWhS_BE.RoutingSupplierInfo READONLY
	
AS
BEGIN

	Declare @EffectiveTime_local DateTime = @EffectiveTime
	Declare @IsFuture_local bit = @IsFuture
	Declare @ActiveSuppliersInfo_local TOneWhS_BE.RoutingSupplierInfo
	Insert into @ActiveSuppliersInfo_local select * from @ActiveSuppliersInfo

	SELECT  sr.[ID],sr.Rate,sr.PriceListID,sr.RateTypeID,sr.ZoneID,sr.BED,sr.EED
	FROM	[TOneWhS_BE].SupplierRate sr WITH(NOLOCK)   
			LEFT JOIN [TOneWhS_BE].SupplierZone sz WITH(NOLOCK) ON sr.ZoneID = sz.ID 
			LEFT JOIN @ActiveSuppliersInfo_local s on s.SupplierId = sz.SupplierId
	Where	(@IsFuture_local = 0 AND sr.BED <= @EffectiveTime_local AND (sr.EED > @EffectiveTime_local OR sr.EED IS NULL))
			OR
			(@IsFuture_local = 1 AND (sr.BED > GETDATE() OR sr.EED IS NULL))
END