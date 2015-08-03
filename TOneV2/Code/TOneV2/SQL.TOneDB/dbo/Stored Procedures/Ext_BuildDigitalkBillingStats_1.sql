

CREATE PROCEDURE [dbo].[Ext_BuildDigitalkBillingStats]
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
DECLARE @minid bigint
DECLARE @maxid bigint
 
SET @FromDate = dbo.DateOf(@Day)

SET @ToDate = dateadd(dd, 1, @FromDate)
PRINT 'Begin Delete on ' + cast(@FromDate AS varchar(20))
PRINT CONVERT(varchar,getdate(),121)
	
-----------------------------
-- Delete Billing Stats
-----------------------------
--EXEC bp_CleanBillingStats @Date = @FromDate, @CustomerID = @CustomerID, @Batch = @Batch

PRINT 'Begin Insert on ' + cast(@FromDate AS varchar(20))
PRINT CONVERT(varchar,getdate(),121)
	
BEGIN TRANSACTION 

Delete Ext_DigitalkBilling_Stats Where CallDate>=@FromDate and CallDate<=@ToDate
	
set @minid = (select min(id) from billing_cdr_main
with (nolock , index = ix_billing_cdr_main_attempt)
WHERE  Attempt BETWEEN @FromDate AND @ToDate)

set @maxid = (select max(id) from billing_cdr_main
with (nolock , index = ix_billing_cdr_main_attempt)
WHERE  Attempt BETWEEN @FromDate AND @ToDate)
    
    INSERT INTO Ext_DigitalkBilling_Stats
    (
        CallDate, CustomerID, SupplierID, CostZoneID, SaleZoneID, Cost_Currency, 
        Sale_Currency, SaleDuration,CostDuration, NumberOfCalls, FirstCallTime, LastCallTime, 
        MinDuration, MaxDuration, AvgDuration, Cost_Nets, Cost_Discounts, 
        Cost_Commissions, Cost_ExtraCharges, Sale_Nets, Sale_Discounts, 
        Sale_Commissions, Sale_ExtraCharges, Sale_Rate, Cost_Rate,Sale_RateType,
        Cost_RateType,AgentPrefix
    )
    
    SELECT DATEADD(day,0,datediff(day,0, bcm.Attempt)),
           bcm.CustomerID,
           bcm.SupplierID,
           bcm.SupplierZoneID,
           bcm.OurZoneID,
           bcc.CurrencyID,
           bcs.CurrencyID,
           SUM(bcs.DurationInSeconds),
           SUM(bcc.DurationInSeconds),
           COUNT(*),
           convert(varchar(5),MIN(bcm.Attempt),108),
           convert(varchar(5),MAX(bcm.Attempt),108),
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
           bcc.RateType,
		   substring(bcm.Extra_Fields,8,4)
    FROM   Billing_CDR_Main bcm WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt))
           LEFT JOIN  Billing_CDR_Sale bcs (NOLOCK) ON bcs.ID = bcm.ID
           LEFT JOIN Billing_CDR_Cost bcc (NOLOCK) ON  bcc.ID = bcm.ID
    WHERE  bcm.Attempt BETWEEN @FromDate AND @ToDate
	and bcs.id >= @minid and bcs.id <= @maxid
	and bcc.id >= @minid and bcc.id <= @maxid
	and CustomerID in ('C023','C025','C114' ,'C115')
    GROUP BY
           DATEADD(day,0,datediff(day,0, bcm.Attempt)),
           bcm.CustomerID,
           bcm.SupplierID,
           bcm.SupplierZoneID,
           bcm.OurZoneID,
           bcc.CurrencyID,
           bcs.CurrencyID,
           bcs.RateType,
           bcc.RateType,
		   substring(bcm.Extra_Fields,8,4)

COMMIT 


RETURN