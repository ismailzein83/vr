
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierCode_GetActiveCodesBySuppliers]
	@CodePrefix varchar(20),
	@EffectiveOn DATETIME = NULL,
	@IsFuture BIT,
	@ActiveSuppliersInfo TOneWhS_BE.RoutingSupplierInfo READONLY
AS
BEGIN

	Declare @CodePrefix_local varchar(20) = @CodePrefix
	Declare @EffectiveOn_local DATETIME = @EffectiveOn
	Declare @IsFuture_local BIT = @IsFuture
	Declare @ActiveSuppliersInfo_local TOneWhS_BE.RoutingSupplierInfo

	Insert into @ActiveSuppliersInfo_local select * from @ActiveSuppliersInfo

	SELECT  sc.[ID],sc.Code,sc.ZoneID,sc.BED,sc.EED,sc.CodeGroupID,sc.SourceID
	  FROM [TOneWhS_BE].SupplierCode sc with(nolock) 
	  JOIN [TOneWhS_BE].SupplierZone sz with(nolock) ON sc.ZoneID=sz.ID 
	  JOIN @ActiveSuppliersInfo_local s on s.SupplierId = sz.SupplierId
	  Where Code like @CodePrefix_local + '%' AND
	   ((@IsFuture_local = 0 AND sc.BED <= @EffectiveOn_local AND (sc.EED > @EffectiveOn_local OR sc.EED IS NULL))
	  OR (@IsFuture_local = 1 AND (sc.BED > GETDATE() OR sc.EED IS NULL)))
END