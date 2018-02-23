-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierZoneService_GetEffectiveZoneServicesBySuppliers]
-- Add the parameters for the stored procedure here
@EffectiveTime DATETIME = NULL,
@IsFuture bit,
@SupplierZoneServicesOwners [TOneWhS_BE].[RoutingSupplierInfo] READONLY

AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT ON;


select	szs.[ID], szs.PriceListID, szs.[ZoneID], szs.[PriceListID], szs.SupplierID, szs.[ReceivedServicesFlag], szs.[EffectiveServiceFlag], szs.[BED], szs.[EED], szs.[SourceID]
from	[TOneWhS_BE].SupplierZoneService szs with(nolock)
		--JOIN [TOneWhS_BE].SupplierPriceList spl with(nolock) ON spl.ID = szs.PriceListID
		JOIN @SupplierZoneServicesOwners szso on szso.SupplierId = szs.SupplierId
Where	szs.ZoneID is not null And ((@IsFuture = 0 AND szs.BED <= @EffectiveTime AND (szs.EED > @EffectiveTime OR szs.EED IS NULL))
		OR (@IsFuture = 1 AND (szs.BED > GETDATE() OR szs.EED IS NULL)))
		group by szs.[ID], szs.[ZoneID], szs.[PriceListID], szs.SupplierID, szs.[ReceivedServicesFlag], szs.[EffectiveServiceFlag], szs.[BED], szs.[EED], szs.[SourceID]
END