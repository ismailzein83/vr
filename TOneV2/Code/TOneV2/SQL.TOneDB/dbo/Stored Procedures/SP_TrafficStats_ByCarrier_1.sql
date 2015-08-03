CREATE  PROCEDURE [dbo].[SP_TrafficStats_ByCarrier]
   @CarrierType VARCHAR(10),
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = NULL,
   @SupplierID Varchar(5) = NULL,
   @CustomerAmuID int = NULL,
   @SupplierAmuID int = NULL
AS
BEGIN

IF(@CarrierType = 'Supplier')
EXEC SP_TrafficStats_ByCustomer
	@fromDate = @fromDate,
	@ToDate = @ToDate,
	@SupplierID = @SupplierID,
	@CustomerAmuID = @CustomerAmuID,
	@SupplierAmuID = @SupplierAmuID

IF(@CarrierType = 'Customer')
EXEC SP_TrafficStats_BySupplier
	@fromDate = @fromDate,
	@ToDate = @ToDate,
	@CustomerID = @CustomerID,
	@CustomerAmuID = @CustomerAmuID,
	@SupplierAmuID = @SupplierAmuID

END