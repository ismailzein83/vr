CREATE PROCEDURE dbo.bp_BuildBillingStatsOld
(
	@Day DateTime = NULL,
	@CustomerID VARCHAR(10) = NULL,
	@Batch INT = 5000
)
WITH RECOMPILE
AS
SET NOCOUNT ON

SELECT @Day = ISNULL(@Day, dateadd(dd , -1 , getdate()))

DECLARE @FromDate Datetime
DECLARE @ToDate Datetime 

SET @FromDate = dbo.DateOf(@Day)

SET @ToDate = dateadd(dd, 1, @FromDate)
PRINT 'Begin Delete on ' + cast(@FromDate AS varchar(20))
PRINT CONVERT(varchar,getdate(),121)
	
-----------------------------
-- Delete Billing Stats
-----------------------------
EXEC bp_CleanBillingStats @Date = @FromDate, @CustomerID = @CustomerID, @Batch = @Batch

PRINT 'Begin Insert on ' + cast(@FromDate AS varchar(20))
PRINT CONVERT(varchar,getdate(),121)
	
BEGIN TRANSACTION 
	
IF @CustomerID IS NULL
BEGIN
    
    INSERT INTO Billing_Stats
    (
        CallDate, CustomerID, SupplierID, CostZoneID, SaleZoneID, Cost_Currency, 
        Sale_Currency, SaleDuration,CostDuration, NumberOfCalls, FirstCallTime, LastCallTime, 
        MinDuration, MaxDuration, AvgDuration, Cost_Nets, Cost_Discounts, 
        Cost_Commissions, Cost_ExtraCharges, Sale_Nets, Sale_Discounts, 
        Sale_Commissions, Sale_ExtraCharges, Sale_Rate, Cost_Rate,Sale_RateType,
        Cost_RateType
    )
    
    SELECT dbo.DateOf(bcm.Attempt),
           bcm.CustomerID,
           bcm.SupplierID,
           bcm.SupplierZoneID,
           bcm.OurZoneID,
           bcc.CurrencyID,
           bcs.CurrencyID,
           SUM(bcs.DurationInSeconds),
           SUM(bcc.DurationInSeconds),
           COUNT(*),
           dbo.GetTimePart(MIN(bcm.Attempt)),
           dbo.GetTimePart(MAX(bcm.Attempt)),
           MIN(bcm.DurationInSeconds),
           MAX(bcm.DurationInSeconds),
           AVG(bcm.DurationInSeconds),
           SUM(bcc.Net),
           SUM(ISNULL(bcc.Discount,0)),
           SUM(ISNULL(bcc.CommissionValue,0)),
           SUM(ISNULL(bcc.ExtraChargeValue,0)),
           SUM(bcs.Net),
           SUM(ISNULL(bcs.Discount,0)),
           SUM(ISNULL(bcs.CommissionValue,0)),
           SUM(ISNULL(bcs.ExtraChargeValue,0)),
           AVG(ISNULL(bcs.RateValue,0)),
           AVG(ISNULL(bcc.RateValue,0)),
           bcs.RateType,
           bcc.RateType
    FROM   Billing_CDR_Main bcm WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt))
           LEFT JOIN  Billing_CDR_Sale bcs (NOLOCK) ON bcs.ID = bcm.ID
           LEFT JOIN Billing_CDR_Cost bcc (NOLOCK) ON  bcc.ID = bcm.ID
    WHERE  bcm.Attempt BETWEEN @FromDate AND @ToDate
    GROUP BY
           dbo.DateOf(bcm.Attempt),
           bcm.CustomerID,
           bcm.SupplierID,
           bcm.SupplierZoneID,
           bcm.OurZoneID,
           bcc.CurrencyID,
           bcs.CurrencyID,
           bcs.RateType,
           bcc.RateType
END
ELSE 
BEGIN
    
    INSERT INTO Billing_Stats
    (
        CallDate, CustomerID, SupplierID, CostZoneID, SaleZoneID, Cost_Currency, 
        Sale_Currency, SaleDuration, CostDuration, NumberOfCalls, FirstCallTime, LastCallTime, 
        MinDuration, MaxDuration, AvgDuration, Cost_Nets, Cost_Discounts, 
        Cost_Commissions, Cost_ExtraCharges, Sale_Nets, Sale_Discounts, 
        Sale_Commissions, Sale_ExtraCharges, Sale_Rate, Cost_Rate,Sale_RateType,
        Cost_RateType
    )
    
    SELECT dbo.DateOf(bcm.Attempt),
           bcm.CustomerID,
           bcm.SupplierID,
           bcm.SupplierZoneID,
           bcm.OurZoneID,
           bcc.CurrencyID,
           bcs.CurrencyID,
           SUM(bcs.DurationInSeconds),
           SUM(bcc.DurationInSeconds),
           COUNT(*),
           dbo.GetTimePart(MIN(bcm.Attempt)),
           dbo.GetTimePart(MAX(bcm.Attempt)),
           MIN(bcm.DurationInSeconds),
           MAX(bcm.DurationInSeconds),
           AVG(bcm.DurationInSeconds),
           SUM(bcc.Net),
           SUM(ISNULL(bcc.Discount,0)),
           SUM(ISNULL(bcc.CommissionValue,0)),
           SUM(ISNULL(bcc.ExtraChargeValue,0)),
           SUM(bcs.Net),
           SUM(ISNULL(bcs.Discount,0)),
           SUM(ISNULL(bcs.CommissionValue,0)),
           SUM(ISNULL(bcs.ExtraChargeValue,0)),
           AVG(ISNULL(bcs.RateValue,0)),
           AVG(ISNULL(bcc.RateValue,0)),
           bcs.RateType,
           bcc.RateType
    FROM   Billing_CDR_Main bcm WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt,IX_Billing_CDR_Main_Customer))
           LEFT JOIN Billing_CDR_Sale bcs (NOLOCK) ON bcs.ID = bcm.ID
           LEFT JOIN Billing_CDR_Cost bcc (NOLOCK) ON bcc.ID = bcm.ID
    WHERE  bcm.Attempt BETWEEN @FromDate AND @ToDate
      AND  bcm.CustomerID = @CustomerID
    GROUP BY
           dbo.DateOf(bcm.Attempt),
           bcm.CustomerID,
           bcm.SupplierID,
           bcm.SupplierZoneID,
           bcm.OurZoneID,
           bcc.CurrencyID,
           bcs.CurrencyID,
           bcs.RateType,
           bcc.RateType
END 
COMMIT 
   PRINT 'Finished'
   PRINT CONVERT(varchar,getdate(),121)
     
RETURN