
CREATE PROCEDURE [dbo].[bp_BuildBillingStats]
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

    With    bcm as(Select  bcm.ID,Attempt,bcm.CustomerID,bcm.SupplierID,bcm.SupplierZoneID,bcm.OurZoneID,DurationInSeconds
    FROM    Billing_CDR_Main bcm WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt))
    WHERE    bcm.Attempt >= @FromDate AND bcm.Attempt < @ToDate
    )
    ,bcs as(Select ID,CurrencyID,DurationInSeconds,Discount,Net,CommissionValue,ExtraChargeValue,RateValue,RateType FROM   Billing_CDR_Sale bcs WITH(NOLOCK, INDEX(IX_Billing_CDR_Sale_Attempt))
    WHERE    bcs.Attempt >= @FromDate AND bcs.Attempt < @ToDate
    )    
    ,bcc as(Select ID,CurrencyID,DurationInSeconds,Discount,Net,CommissionValue,ExtraChargeValue,RateValue,RateType FROM   Billing_CDR_Cost bcc WITH(NOLOCK, INDEX(IX_Billing_CDR_Cost_Attempt))
    WHERE    bcc.Attempt >= @FromDate AND bcc.Attempt < @ToDate
    )    
   
    INSERT INTO Billing_Stats
    (
        CallDate, CustomerID, SupplierID, CostZoneID, SaleZoneID, Cost_Currency,
        Sale_Currency, SaleDuration,CostDuration, NumberOfCalls, FirstCallTime, LastCallTime,
        MinDuration, MaxDuration, AvgDuration, Cost_Nets, Cost_Discounts,
        Cost_Commissions, Cost_ExtraCharges, Sale_Nets, Sale_Discounts,
        Sale_Commissions, Sale_ExtraCharges, Sale_Rate, Cost_Rate,Sale_RateType,
        Cost_RateType
    )
   
    SELECT    DATEADD(day,0,datediff(day,0, bcm.Attempt)),
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
            bcc.RateType
    FROM    bcm
            LEFT JOIN  bcs ON bcs.ID = bcm.ID
            LEFT JOIN  bcc ON  bcc.ID = bcm.ID 
    WHERE    bcm.Attempt >= @FromDate AND bcm.Attempt < @ToDate
        AND (bcm.id<= (SELECT MAX(ID) FROM  bcs)
    OR bcm.id<= (SELECT MAX(ID) FROM  bcc))
    GROUP BY
           DATEADD(day,0,datediff(day,0, bcm.Attempt)),
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
   
    With bcm as(Select  bcm.ID,Attempt,bcm.CustomerID,bcm.SupplierID,bcm.SupplierZoneID,bcm.OurZoneID,DurationInSeconds
    FROM   Billing_CDR_Main bcm WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt))
    WHERE bcm.Attempt >= @FromDate AND bcm.Attempt < @ToDate AND  CustomerID = @CustomerID
    )
    ,bcs as(Select ID,CurrencyID,DurationInSeconds,Discount,Net,CommissionValue,ExtraChargeValue,RateValue,RateType FROM   Billing_CDR_Sale bcs WITH(NOLOCK, INDEX(IX_Billing_CDR_Sale_Attempt))
    WHERE bcs.Attempt >= @FromDate AND bcs.Attempt < @ToDate
    )    
    ,bcc as(Select ID,CurrencyID,DurationInSeconds,Discount,Net,CommissionValue,ExtraChargeValue,RateValue,RateType FROM   Billing_CDR_Cost bcc WITH(NOLOCK, INDEX(IX_Billing_CDR_Cost_Attempt))
    WHERE bcc.Attempt >= @FromDate AND bcc.Attempt < @ToDate
    )

    INSERT INTO Billing_Stats
    (
        CallDate, CustomerID, SupplierID, CostZoneID, SaleZoneID, Cost_Currency,
        Sale_Currency, SaleDuration, CostDuration, NumberOfCalls, FirstCallTime, LastCallTime,
        MinDuration, MaxDuration, AvgDuration, Cost_Nets, Cost_Discounts,
        Cost_Commissions, Cost_ExtraCharges, Sale_Nets, Sale_Discounts,
        Sale_Commissions, Sale_ExtraCharges, Sale_Rate, Cost_Rate,Sale_RateType,
        Cost_RateType
    )   
    SELECT    DATEADD(day,0,datediff(day,0, bcm.Attempt)),
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
            bcc.RateType
    FROM    bcm
            LEFT JOIN  bcs  ON bcs.ID = bcm.ID
            LEFT JOIN  bcc  ON bcc.ID = bcm.ID 
    WHERE    bcm.Attempt >= @FromDate AND bcm.Attempt < @ToDate
            AND  bcm.CustomerID = @CustomerID
                AND (bcm.id<= (SELECT MAX(ID) FROM  bcs)
    OR bcm.id<= (SELECT MAX(ID) FROM  bcc))
    GROUP BY
           DATEADD(day,0,datediff(day,0, bcm.Attempt)),
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