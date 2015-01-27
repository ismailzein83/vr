CREATE  PROCEDURE [dbo].[SP_TrafficStats_CarriersByZone]
   @CarrierType VARCHAR(10),
   @fromDate datetime,
   @ToDate datetime,
   @ZoneID INT 
AS

BEGIN
	IF(@CarrierType = 'Supplier')
	EXEC SP_TrafficStats_SuppliersByZone
		@fromDate = @fromDate,
		@ToDate = @ToDate,
		@ZoneID = @ZoneID
		
    IF(@CarrierType = 'Customer')
	EXEC SP_TrafficStats_CustomersByZone
		@fromDate = @fromDate,
		@ToDate = @ToDate,
		@ZoneID = @ZoneID

END