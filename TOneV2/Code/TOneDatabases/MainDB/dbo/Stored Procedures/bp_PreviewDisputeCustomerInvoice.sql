


CREATE  PROCEDURE [dbo].[bp_PreviewDisputeCustomerInvoice]
(
	@CustomerID varchar(5),
	@GMTShifttime int,
	@TimeZoneInfo VARCHAR(MAX),
	@FromDate Datetime,
	@ToDate Datetime,
	@IssueDate Datetime,
	@DueDate Datetime
)
With recompile
AS
SET NOCOUNT ON 

-- Creating the Invoice 	
DECLARE @Amount float 
DECLARE @Duration NUMERIC(19,6) 
DECLARE @NumberOfCalls INT 
DECLARE @CurrencyID varchar(3) 

--- dummy data to create table schemas
SELECT TOP 1 * INTO #Billing_Invoice  FROM Billing_Invoice bi WITH(NOLOCK) WHERE 1=0
SELECT TOP 1 * INTO #Billing_Invoice_Details  FROM Billing_Invoice_Details bi  WITH(NOLOCK) WHERE 1=0
-----------------------------------------

DECLARE @From SMALLDATETIME
SET @From = @FromDate

DECLARE @To SMALLDATETIME
SET @To = @ToDate
SET @FromDate = DATEADD(day,0,datediff(day,0, @FromDate))
SET @ToDate = DATEADD(s,-1,DATEADD(day,1,datediff(day,0, @ToDate)))


-- Apply the Time Shift 
SET @FromDate = dateadd(mi,-@GMTShifttime,@FromDate);
SET @ToDate = dateadd(mi,-@GMTShifttime,@ToDate);

--DECLARE @MinId AS BIGINT
--DECLARE @MaxId AS BIGINT

--SELECT @MinId=min(bcm.ID),@MaxId=MAX(bcm.Id)
--FROM   Billing_CDR_Main bcm WITH(NOLOCK)
--WHERE  bcm.Attempt >= @FromDate AND bcm.Attempt <@ToDate AND bcm.CustomerID = @CustomerID;

-- building a temp table that contains all billing data requested 
--------------------------------------------------------------------------------------
;WITH BillingMainCTE AS 
(
  SELECT
    bcm.ID AS ID,
  	bcm.OurZoneID AS OurZoneID,
  	bcm.Attempt AS Attempt
  FROM
  	[DATABASENAME].dbo.Billing_CDR_Main bcm WITH(NOLOCK)
  WHERE bcm.Attempt >= @FromDate
  AND  bcm.Attempt < @ToDate
  AND  bcm.CustomerID = @CustomerID 	
)
,SaleCTE AS (SELECT * FROM [DATABASENAME].dbo.Billing_CDR_Sale bcs  WITH(NOLOCK)
  WHERE bcs.Attempt >= @FromDate
  AND  bcs.Attempt < @ToDate )

INSERT INTO  #Billing_Invoice_Details
(
    InvoiceID, Destination, FromDate, TillDate, Duration, Rate, RateType, Amount, NumberOfCalls, CurrencyID, UserID
)
SELECT 
       1,
       cast (bcm.OurZoneID AS CHAR(100)) COLLATE SQL_Latin1_General_CP1256_CI_AS ,
       MIN(dateadd(mi,@GMTShifttime,Bcm.Attempt)),
       MAX(dateadd(mi,@GMTShifttime,Bcm.Attempt)),
       SUM(Bcs.DurationInSeconds)/60.0,
       Round(Bcs.RateValue,5) ,
       Bcs.RateType,
       ISNULL(Round(SUM(Bcs.Net),2),0),
       COUNT(*),
       Bcs.CurrencyID,
       -1
FROM   BillingMainCTE bcm WITH(NOLOCK)
       LEFT JOIN SaleCTE bcs WITH(NOLOCK) ON  bcs.ID = bcm.ID  AND  bcs.CurrencyID IS NOT NULL
GROUP BY
        cast (bcm.OurZoneID AS CHAR(100)),
        Round(Bcs.RateValue,5) ,
		Bcs.RateType,
		Bcs.CurrencyID



-- Apply the Time Shift 
SET @FromDate = dateadd(mi,@GMTShifttime,@FromDate);
SET @ToDate = dateadd(mi,@GMTShifttime,@ToDate);
-- getting the currency exchange rate for the selected customer  
----------------------------------------------------------
SELECT @CurrencyID = CurrencyID
FROM   CarrierProfile WITH(NOLOCK)
WHERE  ProfileID = (
           SELECT ProfileID
           FROM   CarrierAccount WITH(NOLOCK)
           WHERE  CarrierAccountID = @CustomerID
       )	
         	  
-- Exchange rates 
-----------------------------------------------------------
create table #ExchangeRates (
			Currency VARCHAR(3) COLLATE SQL_Latin1_General_CP1256_CI_AS ,
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO #ExchangeRates 
SELECT gder1.Currency,
       gder1.Date,
       gder1.Rate / gder2.Rate
FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder1  
       LEFT JOIN dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder2  
            ON  gder1.Date = gder2.Date
            AND gder2.Currency = @CurrencyID
            


SELECT 
       @Duration = SUM(Duration),
       @Amount = SUM(Amount),
       @NumberOfCalls =  SUM(NumberOfCalls)
FROM  #Billing_Invoice_Details WITH(NOLOCK)

IF(@Duration IS NULL OR @Amount IS NULL) RETURN



-- saving the billing invoice 
INSERT INTO #Billing_Invoice
(
    BeginDate, EndDate, IssueDate, DueDate, CustomerID, SupplierID, SerialNumber, 
    Duration, Amount, NumberOfCalls, CurrencyID, IsLocked, IsPaid, UserID
)

SELECT @From,
       @To,
       @IssueDate,
       @DueDate,
       @CustomerID,
       'SYS',
       '1',
       0,	--ISNULL(@Duration,0)/60.0,
       0,	--ISNULL(@Amount,0),
       @NumberOfCalls,
       @CurrencyID,
       'N',
       'N',
       -1
       

            
SELECT @Duration = SUM(bid.Duration),
       @NumberOfCalls = SUM(bid.NumberOfCalls)
FROM   #Billing_Invoice_Details bid  WITH(NOLOCK)


SELECT @Amount = SUM(B.amount / ISNULL(ERS.Rate, 1)) 
FROM #Billing_Invoice_Details B WITH(NOLOCK)
LEFT JOIN #ExchangeRates ERS WITH(NOLOCK) ON ERS.Currency = B.CurrencyID AND ERS.Date = DATEADD(day,0,datediff(day,0, B.TillDate))

UPDATE #Billing_Invoice
SET    Duration = ISNULL(@Duration,0),
       Amount = ISNULL(@Amount,0) ,
       NumberOfCalls  = ISNULL(@NumberOfCalls,0) 
       
       -- updating the ZoneName 
UPDATE #Billing_Invoice_Details
SET    Destination = Zone.Name
FROM   #Billing_Invoice_Details BID WITH(NOLOCK),
       Zone WITH(NOLOCK)
WHERE  BID.Destination  = Zone.ZoneID



SELECT * FROM #Billing_Invoice
SELECT * FROM #Billing_Invoice_Details WHERE CurrencyId IS NOT NULL 



RETURN