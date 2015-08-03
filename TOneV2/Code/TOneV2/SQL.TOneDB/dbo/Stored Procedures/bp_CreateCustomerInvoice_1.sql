

CREATE PROCEDURE [dbo].[bp_CreateCustomerInvoice]
(
	@CustomerID varchar(5),
	@CustomerMaskID VARCHAR(10),
	@Serial varchar(50),
	@GMTShifttime int,
	@TimeZoneInfo VARCHAR(MAX),
	@FromDate Datetime,
	@ToDate Datetime,
	@IssueDate Datetime,
	@DueDate Datetime,
	@UserID INT,
	@InvoiceID int output
)
With recompile
AS
SET NOCOUNT ON 

-- Creating the Invoice 	
DECLARE @Amount float 
DECLARE @Duration NUMERIC(19,6) 
DECLARE @NumberOfCalls INT 
DECLARE @CurrencyID varchar(3) 

DECLARE @From SMALLDATETIME
SET @From = @FromDate

DECLARE @To SMALLDATETIME
SET @To = @ToDate

SET @FromDate = DATEADD(day,0,datediff(day,0, @FromDate))
SET @ToDate = DATEADD(s,-1,DATEADD(day,1,datediff(day,0, @ToDate)))


-- check if an invoice is issued before for the same customer and for the same period 
DECLARE @OldSerial varchar(50)
DECLARE @OldNotes VARCHAR(MAX)
SELECT @InvoiceID = bi.InvoiceID,
       @OldSerial = bi.SerialNumber,
       @OldNotes = bi.InvoicePrintedNote
FROM   Billing_Invoice bi (NOLOCK)
WHERE  bi.CustomerID = @CustomerID 
AND 
     (
	     bi.BeginDate BETWEEN @FromDate AND DATEADD(day,0,datediff(day,0, @ToDate))
       OR 
         bi.EndDate BETWEEN @FromDate AND DATEADD(day,0,datediff(day,0, @ToDate))
       )


-- Apply the Time Shift 
SET @FromDate = dateadd(mi,-@GMTShifttime,@FromDate);
SET @ToDate = dateadd(mi,-@GMTShifttime,@ToDate);

-- building a temp table that contains all billing data requested 
--------------------------------------------------------------------------------------

DECLARE @Pivot SMALLDATETIME
SET @Pivot = @FromDate

    SELECT top 0
	   bcm.Attempt,
	   bcm.Connect,
       bcm.CustomerID, bcm.OurZoneID,
       bcm.SupplierID, bcm.SupplierZoneID, bcs.ZoneID, bcs.Net, bcs.CurrencyID,
       bcs.RateValue, bcs.RateType, bcs.DurationInSeconds,
       DATEPART(mm, bcm.[Connect]) AS MonthNumber,
  	   DATEPART(yyyy, bcm.[Connect]) AS YearNumber
   INTO   #TempBillingCDR
FROM   Billing_CDR_Main bcm WITH(NOLOCK)
       LEFT JOIN Billing_CDR_Sale bcs (NOLOCK) ON  bcs.ID = bcm.ID

;WITH BillingMainCTE AS 
(
  SELECT
    bcm.ID AS ID,
    bcm.CustomerID AS CustomerID,
    bcm.SupplierID AS SupplierID,
    bcm.SupplierZoneID AS SupplierZoneID,
      	bcm.OurZoneID AS OurZoneID,
  	bcm.Attempt AS Attempt,
  	bcm.[Connect] AS Connect

  FROM
  	Billing_CDR_Main bcm WITH(NOLOCK)
  WHERE bcm.Attempt >= @FromDate
  AND  bcm.Attempt <= @ToDate
  AND  bcm.CustomerID = @CustomerID 	
)

--,BillingMainIDCTE AS 
--(
--  SELECT
--  	bcm.MonthNumber AS MonthNumber,
--    bcm.YearNumber AS YearNumber,
--    Min(bcm.ID) AS MinID,
--    Max(bcm.ID) AS MaxID
--  FROM BillingMainCTE bcm
--  GROUP BY bcm.MonthNumber,bcm.YearNumber
--)
,Billing_CDR_SaleCTE AS 
(
  SELECT
       * ,  	DATEPART(mm, bcs.[Attempt]) AS MonthNumber,
  	DATEPART(yyyy, bcs.[Attempt]) AS YearNumber 
  FROM Billing_CDR_Sale bcs WITH(NOLOCK) 
WHERE 
	 bcs.ID in(select id from BillingMainCTE)
	
)

INSERT INTO  #TempBillingCDR
    SELECT 
	   bcm.Attempt,
	   bcm.Connect,
       bcm.CustomerID, bcm.OurZoneID,
       bcm.SupplierID, bcm.SupplierZoneID, bcs.ZoneID, bcs.Net, bcs.CurrencyID,
       bcs.RateValue, bcs.RateType, bcs.DurationInSeconds,bcs.MonthNumber,bcs.YearNumber  
FROM   BillingMainCTE bcm WITH(NOLOCK)
       LEFT JOIN Billing_CDR_SaleCTE bcs (NOLOCK) ON  bcs.ID = bcm.ID
WHERE  
    bcs.CurrencyID IS NOT NULL


--UPDATE #TempBillingCDR
--SET Attempt =  dateadd(mi,@GMTShifttime,Attempt),
--    Connect =  dateadd(mi,@GMTShifttime,Connect)

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
DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates 
SELECT gder1.Currency,
       gder1.Date,
       gder1.Rate / gder2.Rate
FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder1 
       LEFT JOIN dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder2
            ON  gder1.Date = gder2.Date
            AND gder2.Currency = @CurrencyID
            


SELECT 
       @Duration = SUM(DurationInSeconds)/60.0,
       @Amount = SUM(Net),
       @NumberOfCalls =  COUNT(*)
FROM  #TempBillingCDR

IF(@Duration IS NULL OR @Amount IS NULL) RETURN

BEGIN TRANSACTION;     
	
IF @InvoiceID IS NOT NULL
BEGIN
    DELETE Billing_Invoice_Costs
    FROM   Billing_Invoice_Costs WITH(NOLOCK,INDEX(IX_Billing_Invoice_Costs_Invoice))
    WHERE  InvoiceID = @InvoiceID
    
    DELETE Billing_Invoice_Details
    FROM   Billing_Invoice_Details WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice))
    WHERE  InvoiceID = @InvoiceID
    
    DELETE Billing_Invoice
    FROM   Billing_Invoice WITH(NOLOCK,INDEX(PK_Billing_Invoice))
    WHERE  InvoiceID = @InvoiceID
    
    SET @Serial = @OldSerial
END

-- saving the billing invoice 
INSERT INTO Billing_Invoice
(
    BeginDate, EndDate, IssueDate, DueDate, CustomerID, SupplierID, SerialNumber, InvoicePrintedNote,
    Duration, Amount, NumberOfCalls, CurrencyID, IsLocked, IsPaid, UserID ,CustomerMask
)

SELECT @From,
       @To,
       @IssueDate,
       @DueDate,
       @CustomerID,
       'SYS',
       @Serial,
       @OldNotes,
       0,	--ISNULL(@Duration,0)/60.0,
       0,	--ISNULL(@Amount,0),
       @NumberOfCalls,
       @CurrencyID,
       'N',
       'N',
       @UserID,
       @CustomerMaskID 
       
       -- Getting the Invoice ID saved 
SELECT @InvoiceID = @@IDENTITY 

-- Getting the Billing Invoice Details 

INSERT INTO Billing_Invoice_Details
(
    InvoiceID, Destination, FromDate, TillDate, Duration, Rate, RateType, Amount, NumberOfCalls, CurrencyID, UserID,YearNumber
)
SELECT 
       @InvoiceID,
       B.ZoneID,
       dateadd(mi,@GMTShifttime,MIN(B.Attempt)),
       dateadd(mi,@GMTShifttime,MAX(B.Attempt)),
       SUM(B.DurationInSeconds)/60.0,
       Round(B.RateValue,5) ,
       B.RateType,
       ISNULL(Round(SUM(B.Net),2),0),
       COUNT(*),
       B.CurrencyID,
       @UserID,
      -- B.MonthNumber,
       B.YearNumber
FROM   #TempBillingCDR B
GROUP BY
       B.ZoneID,
       --B.MonthNumber,
       B.YearNumber,
       Round(B.RateValue,5),
       B.RateType,
       B.CurrencyID
       

            
SELECT @Duration = SUM(bid.Duration),
       @NumberOfCalls = SUM(bid.NumberOfCalls)
FROM   Billing_Invoice_Details bid WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice))
WHERE  bid.InvoiceID = @InvoiceID

SELECT @Amount = ISNULL(Round(SUM(B.Net / ISNULL(ERS.Rate, 1)),2),0)  
FROM #TempBillingCDR B
LEFT JOIN @ExchangeRates ERS ON ERS.Currency = B.CurrencyID AND ERS.Date = DATEADD(day,0,datediff(day,0, B.Attempt))

UPDATE Billing_Invoice
SET    Duration = ISNULL(@Duration,0),
       Amount = ISNULL(@Amount,0) ,
       NumberOfCalls  = ISNULL(@NumberOfCalls,0) 
WHERE  InvoiceID = @InvoiceID

-- calculating the billing invoice costs (Gouped by the Out Carrier 'Supplier') 
INSERT INTO Billing_Invoice_Costs
(
    InvoiceID, SupplierID, Destination, FromDate, TillDate, Duration, Rate, RateType, Amount, NumberOfCalls, CurrencyID, UserID
)      
SELECT 
       @InvoiceID,
       B.SupplierID,
       B.SupplierZoneID,
       MIN(B.Attempt),
       MAX(B.Attempt),
       SUM(B.DurationInSeconds)/60.0,
       Round(B.RateValue,5),
       B.RateType,
       ISNULL(Round(SUM(B.Net),2),0),
       COUNT(*),
       B.CurrencyID,
       @UserID
FROM    #TempBillingCDR B
GROUP BY
       B.SupplierID,
       B.SupplierZoneID,
       Round(B.RateValue,5),
       B.RateType,
       B.CurrencyID
       
       -- updating the ZoneName 
UPDATE Billing_Invoice_Details
SET    Destination = Zone.Name
FROM   Billing_Invoice_Details WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice)),
       Zone WITH(NOLOCK)
WHERE  
		Billing_Invoice_Details.InvoiceID = @InvoiceID
		AND  Billing_Invoice_Details.Destination = Zone.ZoneID

UPDATE Billing_Invoice_Costs
SET    Destination = Zone.Name
FROM   Billing_Invoice_Costs WITH(NOLOCK,INDEX(IX_Billing_Invoice_Costs_Invoice)),
       Zone WITH(NOLOCK)
WHERE  Billing_Invoice_Costs.InvoiceID = @InvoiceID
	   And Billing_Invoice_Costs.Destination = Zone.ZoneID

UPDATE Billing_Invoice
SET InvoiceNotes = @TimeZoneInfo WHERE InvoiceID = @InvoiceID

  -- Rollback the transaction if there were any errors
IF @@ERROR <> 0
BEGIN
    -- Rollback the transaction
    ROLLBACK
    
    -- Raise an error and return
    -- RAISERROR ('Error Creating Invoice', 16, 1)
    RETURN
END
COMMIT
	RETURN