
CREATE  PROCEDURE [dbo].[SP_TrafficStats_ZoneCarriers]
   @CarrierType VARCHAR(10),
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = NULL,
   @SupplierID Varchar(5) = NULL,
   @ZoneID INT,
   @CustomerAmuID int = NULL,
   @SupplierAmuID int = NULL
AS
BEGIN
	
	IF(@CarrierType = 'Supplier')
	EXEC SP_TrafficStats_ZoneCustomers
		@fromDate = @fromDate,
		@ToDate = @ToDate,
		@SupplierID = @SupplierID,
		@ZoneID = @ZoneID,
		@CustomerAmuID = @CustomerAmuID,
		@SupplierAmuID = @SupplierAmuID
		
    IF(@CarrierType = 'Customer')
	EXEC SP_TrafficStats_ZoneSuppliers
		@fromDate = @fromDate,
		@ToDate = @ToDate,
		@CustomerID = @CustomerID,
		@ZoneID = @ZoneID,
		@CustomerAmuID = @CustomerAmuID,
		@SupplierAmuID = @SupplierAmuID

END