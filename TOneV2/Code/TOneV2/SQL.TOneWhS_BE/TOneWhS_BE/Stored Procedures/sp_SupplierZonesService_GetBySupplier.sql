-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierZonesService_GetBySupplier] 
	@SupplierId_FromOut INT,
	@FromDate_FromOut DateTime,
	@ToDate_FromOut DateTime
AS
BEGIN
	DECLARE @SupplierId INT
	DECLARE @FromDate DateTime
	DECLARE @ToDate DateTime

	SELECT @FromDate = @FromDate_FromOut
	SELECT @ToDate = @ToDate_FromOut
	SELECT @SupplierId = @SupplierId_FromOut

	SELECT  [ID],[PriceListID],[ZoneID], [SupplierID],[ReceivedServicesFlag],[EffectiveServiceFlag],[BED],[EED]
	FROM	[TOneWhS_BE].SupplierZoneService szs WITH(NOLOCK) 
	Where   BED <= @ToDate 
			AND	(EED is null or EED >@FromDate )
			AND SupplierID = @SupplierId
			AND ZoneID is not null
END