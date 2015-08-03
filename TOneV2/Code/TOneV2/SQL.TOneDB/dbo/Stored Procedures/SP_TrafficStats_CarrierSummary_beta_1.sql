
CREATE  PROCEDURE [dbo].[SP_TrafficStats_CarrierSummary_beta]
   @CarrierType VARCHAR(10),
   @fromDate datetime ,
   @ToDate datetime,
   @CustomerID varchar(15)=null,
   @SupplierID varchar(15)=null,
   @TopRecord INT =NULL
AS
BEGIN
	
	IF(@CarrierType = 'Supplier')
	 EXEC SP_TrafficStats_SupplierSummary
	 	@fromDate = @fromDate,
	 	@ToDate = @ToDate,
	 	@SupplierID = @SupplierID,
	 	@TopRecord = @TopRecord

    IF(@CarrierType = 'Customer')
	 EXEC SP_TrafficStats_SupplierSummary
	 	@fromDate = @fromDate,
	 	@ToDate = @ToDate,
	 	@CustomerID = @CustomerID,
	 	@TopRecord = @TopRecord

END