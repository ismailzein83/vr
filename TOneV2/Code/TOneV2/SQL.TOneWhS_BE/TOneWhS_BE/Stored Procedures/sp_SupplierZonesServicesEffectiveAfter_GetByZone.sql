-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierZonesServicesEffectiveAfter_GetByZone] 
	@SupplierId INT,
	@EffectiveDate DateTime,
	@StrZoneIds varchar(max)
AS
BEGIN
Declare  @ZoneIdsTable Table (ZoneId bigint)
	INSERT INTO  @ZoneIdsTable (ZoneId)
	select Convert(bigint,ParsedString) FROM [TOneWhS_BE].[ParseStringList](@StrZoneIds)

	SELECT  [ID],[PriceListID],[ZoneID], [SupplierID],[ReceivedServicesFlag],[EffectiveServiceFlag],[BED],[EED]
	FROM	[TOneWhS_BE].SupplierZoneService WITH(NOLOCK) 	
	Where	(EED is null or EED > @EffectiveDate)
			and SupplierID=@SupplierId and ZoneID is not null
			and (@StrZoneIds  is null or ZoneID in (select ZoneId from @ZoneIdsTable))
END