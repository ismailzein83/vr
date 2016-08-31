
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierCode_GetActiveCodesBySuppliers]
	@EffectiveOn DATETIME = NULL,
	@IsFuture BIT,
	@ActiveSuppliersInfo TOneWhS_BE.RoutingSupplierInfo READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT  sc.[ID],sc.Code,sc.ZoneID,sc.BED,sc.EED
	  FROM [TOneWhS_BE].SupplierCode sc with(nolock) 
	  JOIN [TOneWhS_BE].SupplierZone sz with(nolock) ON sc.ZoneID=sz.ID 
	  JOIN @ActiveSuppliersInfo s on s.SupplierId = sz.SupplierId
	  Where 
	   ((@IsFuture = 0 AND sc.BED <= @EffectiveOn AND (sc.EED > @EffectiveOn OR sc.EED IS NULL))
	  OR (@IsFuture = 1 AND (sc.BED > GETDATE() OR sc.EED IS NULL)))
END