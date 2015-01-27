CREATE PROCEDURE [dbo].[bp_CleanBillingStats]
(
	@Date datetime, 
	@SwitchID tinyint = NULL, 
	@CustomerID varchar(10) = NULL, 
	@SupplierID varchar(10) = NULL, 
	@Batch bigint = 500, 
	@IsDebug char(1) = 'N'	
)
AS
-----------------------------
-- Delete Billing Stats
-----------------------------
DECLARE @DeletedCount bigint
SET NOCOUNT ON 
SELECT @DeletedCount = 1
SET ROWCOUNT @Batch
WHILE @DeletedCount > 0
BEGIN
	BEGIN TRANSACTION Cleaner							
		-- No Customer, No Supplier
		IF @CustomerID IS NULL AND @SupplierID IS NULL
			DELETE Billing_Stats FROM Billing_Stats WITH(NOLOCK, INDEX(IX_Billing_Stats_Date)) WHERE CallDate = @Date
		-- No Supplier
		ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
			DELETE Billing_Stats FROM Billing_Stats WITH(NOLOCK, INDEX(IX_Billing_Stats_Date, IX_Billing_Stats_Customer)) WHERE CallDate = @Date AND CustomerID = @CustomerID
		-- Customer, Supplier
		ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL	
			DELETE Billing_Stats FROM Billing_Stats WITH(NOLOCK, INDEX(IX_Billing_Stats_Date, IX_Billing_Stats_Customer, IX_Billing_Stats_Supplier)) WHERE CallDate = @Date AND CustomerID = @CustomerID AND SupplierID = @SupplierID
		-- No Customer
		ELSE
			DELETE Billing_Stats FROM Billing_Stats WITH(NOLOCK, INDEX(IX_Billing_Stats_Date, IX_Billing_Stats_Supplier)) WHERE CallDate = @Date AND SupplierID = @SupplierID
		SET @DeletedCount = @@ROWCOUNT
	COMMIT TRANSACTION Cleaner			
END
IF @IsDebug = 'Y' PRINT 'Deleted Billing Stats ' + convert(varchar(25), getdate(), 121)