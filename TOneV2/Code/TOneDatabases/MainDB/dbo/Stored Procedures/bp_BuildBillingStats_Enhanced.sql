
CREATE PROCEDURE [dbo].[bp_BuildBillingStats_Enhanced]
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
EXEC bp_CleanBillingStats @Date = @FromDate, @CustomerID = @CustomerID, @Batch = @Batch

PRINT 'Begin Insert on ' + cast(@FromDate AS varchar(20))
PRINT CONVERT(varchar,getdate(),121)
	
BEGIN TRANSACTION
IF @CustomerID IS NULL
BEGIN

--set @minid = (select min(id) from billing_cdr_main
--with (nolock , index = ix_billing_cdr_main_attempt)
--WHERE  Attempt BETWEEN @FromDate AND @ToDate)

--set @maxid = (select max(id) from billing_cdr_main
--with (nolock , index = ix_billing_cdr_main_attempt)
--WHERE  Attempt BETWEEN @FromDate AND @ToDate)
;WITH BillingMainCTE AS 
(
	SELECT
	bcm.ID AS ID,
	bcm.CustomerID AS CustomerID,
	bcm.SupplierID AS SupplierID,
	bcm.SupplierZoneID AS SupplierZoneID,
	bcm.OurZoneID AS OurZoneID,
	bcm.Attempt AS Attempt,
	bcm.DurationInSeconds AS DurationInSeconds
	FROM
		Billing_CDR_Main bcm WITH(NOLOCK)
	WHERE bcm.Attempt >= @FromDate AND bcm.Attempt < @ToDate  
),
MainSaleCTE AS 
(
SELECT 	bcm.ID AS ID,
	bcm.CustomerID AS CustomerID,
	bcm.SupplierID AS SupplierID,
	bcm.SupplierZoneID AS SupplierZoneID,
	bcm.OurZoneID AS OurZoneID,
	bcm.Attempt AS Attempt,
	bcm.DurationInSeconds AS DurationInSeconds,
	bcs.DurationInSeconds AS SaleDurationInSeconds,
	bcs.CurrencyID AS SaleCurrency,
    bcs.Net AS SaleNet,
    bcs.Discount AS SaleDiscount,
    bcs.CommissionValue AS SaleCommissionValue,
    bcs.ExtraChargeValue AS SaleExtraChargeValue, 
    bcs.RateType AS SaleRateType,
    bcs.RateValue AS SaleRateValue
FROM 	BillingMainCTE BCM WITH(NOLOCK)
LEFT JOIN Billing_CDR_Sale bcs ON bcs.ID = BCM.id 
  	
)
,MainCostCTE AS 
(
SELECT 	
    bcm.ID AS ID,
	bcs.DurationInSeconds AS CostDurationInSeconds,
	bcs.CurrencyID AS CostCurrency,
    bcs.Net AS CostNet,
    bcs.Discount AS CostDiscount,
    bcs.CommissionValue AS CostCommissionValue,
    bcs.ExtraChargeValue AS CostExtraChargeValue, 
    bcs.RateType AS CostRateType,
    bcs.RateValue AS CostRateValue
FROM 	BillingMainCTE BCM WITH(NOLOCK)
LEFT JOIN Billing_CDR_Cost bcs ON bcs.ID = BCM.id 
  	
)
, AllbillingCTE AS 
(
  SELECT  
  bcm.ID AS ID,
	bcm.CustomerID AS CustomerID,
	bcm.SupplierID AS SupplierID,
	bcm.SupplierZoneID AS SupplierZoneID,
	bcm.OurZoneID AS OurZoneID,
	bcm.Attempt AS Attempt,
	bcm.DurationInSeconds AS DurationInSeconds,
	bcm.SaleDurationInSeconds AS SaleDurationInSeconds,
	bcm.SaleCurrency AS SaleCurrency,
    bcm.SaleNet AS SaleNet,
    bcm.SaleDiscount AS SaleDiscount,
    bcm.SaleCommissionValue AS SaleCommissionValue,
    bcm.SaleExtraChargeValue AS SaleExtraChargeValue, 
    bcm.SaleRateType AS SaleRateType,
    bcm.SaleRateValue AS SaleRateValue,
    bcc.CostDurationInSeconds AS CostDurationInSeconds,
	bcc.CostCurrency AS CostCurrency,
    bcc.CostNet AS CostNet,
    bcc.CostDiscount AS CostDiscount,
    bcc.CostCommissionValue AS CostCommissionValue,
    bcc.CostExtraChargeValue AS CostExtraChargeValue, 
    bcc.CostRateType AS CostRateType,
    bcc.CostRateValue AS CostRateValue
  FROM MainSaleCTE bcm WITH(NOLOCK)
  LEFT JOIN  MainCostCTE bcc ON bcm.Id = bcc.Id	
)

, FinalCTE AS 
(
  	 SELECT --dbo.DateOf(bcm.Attempt) AS Attempt ,
           bcm.CustomerID AS CustomerID
           --bcm.SupplierID SupplierID,
           --bcm.SupplierZoneID SupplierZoneID,
           --bcm.OurZoneID OurZoneID,
           --bcm.SaleCurrency AS SaleCurrency,
           --bcm.CostCurrency CostCurrency,
           --SUM(bcm.SaleDurationInSeconds) SaleDurationInSeconds,
           --SUM(bcm.CostDurationInSeconds) CostDurationInSeconds,
           --COUNT(*) AS Calls,
           --dbo.GetTimePart(MIN(bcm.Attempt)) AS MinDate,
           --dbo.GetTimePart(MAX(bcm.Attempt)) MaxDate,
           --MIN(bcm.DurationInSeconds) AS MinDuration,
           --MAX(bcm.DurationInSeconds) AS MaxDuration,
           --AVG(bcm.DurationInSeconds) AS AverageDuration,
           --SUM(bcm.CostNet) AS  CostNet,
           --SUM(ISNULL(bcm.CostDiscount,0)) AS CostDiscount,
           --SUM(ISNULL(bcm.CostCommissionValue,0)) AS CostCommissionValue,
           --SUM(ISNULL(bcm.CostExtraChargeValue,0)) CostExtraChargeValue,
           --SUM(bcm.SaleNet) SaleNet,
           --SUM(ISNULL(bcm.SaleDiscount,0)) SaleDiscount,
           --SUM(ISNULL(bcm.SaleCommissionValue,0)) SaleCommissionValue,
           --SUM(ISNULL(bcm.SaleExtraChargeValue,0)) SaleExtraChargeValue,
           --AVG(ISNULL(bcm.SaleRateValue,0)) SaleRateValue,
           --AVG(ISNULL(bcm.CostRateValue,0)) CostRateValue,
           --bcm.SaleRateType SaleRateType,
           --bcm.CostRateType CostRateType
    FROM   AllbillingCTE bcm WITH(NOLOCK)
    GROUP BY
          -- dbo.DateOf(bcm.Attempt),
           bcm.CustomerID
           --bcm.SupplierID,
           --bcm.SupplierZoneID,
           --bcm.OurZoneID,
           --bcm.CostCurrency,
           --bcm.SaleCurrency,
           --bcm.SaleRateType,
           --bcm.CostrateType
)
SELECT COUNT(*) FROM FinalCTE
RETURN 

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
    FROM   BillingMainCTE bcm WITH(NOLOCK)
           LEFT JOIN Billing_CDR_Sale  bcs (NOLOCK) ON bcs.ID = bcm.ID --and bcs.id >= @minid and bcs.id <= @maxid
           LEFT JOIN Billing_CDR_Cost bcc (NOLOCK) ON  bcc.ID = bcm.ID --and bcc.id >= @minid and bcc.id <= @maxid 
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
    
set @minid = (select min(id) from billing_cdr_main
with (nolock , index = ix_billing_cdr_main_attempt)
WHERE  Attempt BETWEEN @FromDate  AND @ToDate
AND  CustomerID = @CustomerID)

set @maxid = (select max(id) from billing_cdr_main
with (nolock , index = ix_billing_cdr_main_attempt)
WHERE  Attempt BETWEEN @FromDate AND @ToDate
AND  CustomerID = @CustomerID)

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
    FROM   Billing_CDR_Main bcm WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt))
           LEFT JOIN Billing_CDR_Sale bcs (NOLOCK) ON bcs.ID = bcm.ID and bcs.id >= @minid and bcs.id <= @maxid
           LEFT JOIN Billing_CDR_Cost bcc (NOLOCK) ON bcc.ID = bcm.ID and bcc.id >= @minid and bcc.id <= @maxid 
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