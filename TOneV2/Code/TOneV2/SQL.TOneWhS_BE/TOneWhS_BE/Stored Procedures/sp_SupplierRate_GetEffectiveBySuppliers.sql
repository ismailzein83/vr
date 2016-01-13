﻿

CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetEffectiveBySuppliers]
	@EffectiveTime DATETIME = NULL,
	@IsFuture bit,
	@ActiveSuppliersInfo TOneWhS_BE.RoutingSupplierInfo READONLY
	
AS
BEGIN

	SET NOCOUNT ON;

	SELECT  sr.[ID]
		  ,sr.NormalRate
		  ,sr.OtherRates
		  ,sr.PriceListID
		  ,pl.CurrencyId
		  ,sr.ZoneID
		  ,sr.BED
		  ,sr.EED
	  FROM [TOneWhS_BE].SupplierRate sr 	  
	  JOIN [TOneWhS_BE].SupplierPriceList pl ON sr.PriceListID = pl.ID 
	  JOIN @ActiveSuppliersInfo s on s.SupplierId = pl.SupplierId
	  Where (@IsFuture = 0 AND sr.BED <= @EffectiveTime AND (sr.EED > @EffectiveTime OR sr.EED IS NULL))
			OR
			(@IsFuture = 1 AND (sr.BED > GETDATE() OR sr.EED IS NULL))

END