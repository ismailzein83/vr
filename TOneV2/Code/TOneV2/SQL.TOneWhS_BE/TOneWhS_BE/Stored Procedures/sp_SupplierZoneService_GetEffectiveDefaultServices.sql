-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierZoneService_GetEffectiveDefaultServices] 
	@FromDate_FromOut DateTime,
	@ToDate_FromOut DateTime
AS
BEGIN
	DECLARE @FromDate DateTime
	DECLARE @ToDate DateTime

	SELECT @FromDate = @FromDate_FromOut
	SELECT @ToDate = @ToDate_FromOut

	SELECT  [ID],[PriceListID],[ZoneID], [SupplierID],[ReceivedServicesFlag],[EffectiveServiceFlag],[BED],[EED]
	FROM	[TOneWhS_BE].SupplierZoneService szs WITH(NOLOCK) 
	Where   BED <=  @ToDate
			AND (EED is null or EED > @FromDate) 
			AND  ZoneID is null	  
END