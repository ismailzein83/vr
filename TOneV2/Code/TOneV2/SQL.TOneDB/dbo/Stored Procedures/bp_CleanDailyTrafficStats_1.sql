Create PROC [dbo].[bp_CleanDailyTrafficStats]
(
	@DataTime DATETIME ,
	@CustomerID NVARCHAR(20),
	@SupplierID NVARCHAR(20)
)
AS
IF(@CustomerID IS NOT NULL AND @SupplierID IS NOT null)
BEGIN
	DELETE FROM TrafficStatsDaily WHERE calldate =@DataTime AND CustomerID=@CustomerID AND SupplierID=@SupplierID
END
ELSE
	BEGIN
		IF(@CustomerID IS NOT NULL AND @SupplierID IS null)
		BEGIN
			DELETE FROM TrafficStatsDaily WHERE calldate=@DataTime AND CustomerID=@CustomerID
		END
		IF(@CustomerID IS NULL AND @SupplierID IS NOT null)
		BEGIN
			DELETE FROM TrafficStatsDaily WHERE calldate=@DataTime AND SupplierID=@SupplierID
		END
		IF(@CustomerID IS NULL AND @SupplierID IS null)--Reprising For a FAllDate and For All Customers and Suppliers
		BEGIN
			DELETE FROM TrafficStatsDaily WHERE calldate=@DataTime
		END
END