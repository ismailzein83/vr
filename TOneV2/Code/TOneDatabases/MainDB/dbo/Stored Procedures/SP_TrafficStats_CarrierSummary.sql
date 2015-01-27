CREATE PROCEDURE [dbo].[SP_TrafficStats_CarrierSummary]
   @CarrierType VARCHAR(10),
   @fromDate datetime ,
   @ToDate datetime,
   @CustomerID varchar(15)=null,
   @SupplierID varchar(15)=null,
   @TopRecord INT =NULL,
   @GroupByProfile char(1) = 'N',
   @CustomerAmuID int = NULL,
   @SupplierAmuID int = NULL
AS
BEGIN
	
	IF(@CarrierType = 'Supplier')
	 EXEC SP_TrafficStats_SupplierSummary
	 	@fromDate = @fromDate,
	 	@ToDate = @ToDate,
	 	@SupplierID = @SupplierID,
	 	@TopRecord = @TopRecord,
	 	@GroupByProfile = @GroupByProfile,
	 	@CustomerAmuID = @CustomerAmuID,
		@SupplierAmuID = @SupplierAmuID

    IF(@CarrierType = 'Customer')
	 EXEC SP_TrafficStats_CustomerSummary
	 	@fromDate = @fromDate,
	 	@ToDate = @ToDate,
	 	@CustomerID = @CustomerID,
	 	@TopRecord = @TopRecord,
	 	@GroupByProfile = @GroupByProfile,
	 	@CustomerAmuID = @CustomerAmuID,
		@SupplierAmuID = @SupplierAmuID

END