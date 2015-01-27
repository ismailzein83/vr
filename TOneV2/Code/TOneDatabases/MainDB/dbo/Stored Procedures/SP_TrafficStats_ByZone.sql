
CREATE  PROCEDURE [dbo].[SP_TrafficStats_ByZone]
   @CarrierType VARCHAR(10),  
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = NULL,
   @SupplierID Varchar(5) = null,
   @CustomerAmuID int = NULL,
   @SupplierAmuID int = NULL
AS
BEGIN

IF(@CarrierType = 'Customer')
EXEC SP_TrafficStats_CustomerSaleZone
	@fromDate = @fromDate,
	@ToDate = @ToDate,
	@CustomerID = @CustomerID,
	@CustomerAmuID = @CustomerAmuID,
	@SupplierAmuID = @SupplierAmuID

IF(@CarrierType = 'Supplier')
EXEC SP_TrafficStats_SupplierCostZone
	@fromDate = @fromDate,
	@ToDate = @ToDate,
	@SupplierID =@SupplierID,
	@CustomerAmuID = @CustomerAmuID,
	@SupplierAmuID = @SupplierAmuID

END