

CREATE PROCEDURE [dbo].[bp_CreateInvoiceDispute]
(
	@CustomerID varchar(5),
	@CustomerMaskID VARCHAR(10),
	@Serial varchar(50),
	@FromDate Datetime,
	@ToDate Datetime,
	@IssueDate Datetime,
	@DueDate Datetime,
	@UserID INT,
	@TimeZoneInfo VARCHAR(MAX),
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

SET @FromDate = CAST(
(
STR( YEAR( @FromDate ) ) + '-' +
STR( MONTH( @FromDate ) ) + '-' +
STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate = CAST(
(
STR( YEAR( @ToDate ) ) + '-' +
STR( MONTH(@ToDate ) ) + '-' +
STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)

-- building a temp table that contains all billing data requested 
--------------------------------------------------------------------------------------

DECLARE @Pivot SMALLDATETIME
SET @Pivot = @FromDate

SELECT *, DATEPART(mm,bs.CallDate) AS MonthNumber,DATEPART(yyyy,bs.CallDate) AS YearNumber  INTO #TempBillingStats 
FROM [DATABASENAME].dbo.Billing_Stats bs WITH(NOLOCK) WHERE bs.CallDate='1995-01-01'

WHILE @Pivot <= @ToDate
BEGIN
	
    --------------    
INSERT INTO #TempBillingStats
SELECT  *, DATEPART(mm,bs.CallDate) AS MonthNumber,DATEPART(yyyy,bs.CallDate) AS YearNumber   
FROM [DATABASENAME].dbo.Billing_Stats bs  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
WHERE  BS.CallDate = @Pivot 
	  AND  BS.CustomerID = @CustomerID
	  AND  BS.Sale_Currency IS NOT NULL 
	  
    --------------
SET @Pivot = DATEADD(dd, 1, @Pivot)
END 

--SELECT * FROM #TempBillingStats
-- getting the currency exchange rate for the selected customer  
----------------------------------------------------------
SELECT @CurrencyID = CurrencyID
FROM   CarrierProfile (NOLOCK)
WHERE  ProfileID = (
           SELECT ProfileID
           FROM   CarrierAccount (NOLOCK)
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

-- check if an invoice is issued before for the same customer 
-----------------------------------------------------------
DECLARE @OldSerial varchar(50)
DECLARE @OldNotes VARCHAR(MAX)
SELECT @InvoiceID = bi.InvoiceID,
       @OldSerial = bi.SerialNumber,
       @OldNotes = bi.InvoicePrintedNote
FROM   Billing_Invoice bi WITH(NOLOCK,INDEX(IX_Billing_Invoice_BeginDate,IX_Billing_Invoice_Customer))
WHERE (
	     bi.BeginDate BETWEEN @FromDate AND DATEADD(day,0,datediff(day,0, @ToDate))
       OR 
         bi.EndDate BETWEEN @FromDate AND DATEADD(day,0,datediff(day,0, @ToDate))
       )
  AND bi.CustomerID = @CustomerID

-- Check if there is data in billing stats 
----------------------------------------------------------
SELECT 
       @Duration = SUM(BS.SaleDuration)/60.0,
       @Amount = SUM(BS.Sale_Nets),
       @NumberOfCalls =  SUM(BS.NumberOfCalls)
FROM   #TempBillingStats BS

IF(@Duration IS NULL OR @Amount IS NULL) RETURN


BEGIN TRANSACTION;     

-- get the old invoiceID and the Serial number 
----------------------------------------------------------	
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
-----------------------------------------------------------
INSERT INTO Billing_Invoice
(
    BeginDate, EndDate, IssueDate, DueDate, CustomerID, SupplierID, SerialNumber,InvoicePrintedNote, Duration, 
    Amount, NumberOfCalls, CurrencyID, IsLocked, IsPaid, UserID, CustomerMask
)

SELECT @FromDate,
       @ToDate,
       @IssueDate,
       @DueDate,
       @CustomerID,
       'SYS',
       @Serial,
       @OldNotes,
       0,
       0,
       @NumberOfCalls,
       @CurrencyID,
       'N',
       'N',
       @UserID,
       @CustomerMaskID 
       
-- Getting the Invoice ID saved 
SELECT @InvoiceID = @@IDENTITY 
UPDATE Billing_Invoice
SET    EndDate = dateadd(dd,-1,EndDate)
WHERE  InvoiceID = @InvoiceID


-- Getting the Billing Invoice Details 

INSERT INTO Billing_Invoice_Details
(
    InvoiceID, Destination, FromDate, TillDate, Duration, Rate, RateType, Amount, NumberOfCalls, CurrencyID, UserID,MonthNumber,YearNumber
)
SELECT 
       @InvoiceID,
       BS.SaleZoneID,
       MIN(BS.CallDate),
       MAX(BS.CallDate),
       SUM(BS.SaleDuration)/60.0,
       Round(BS.Sale_Rate,5),
       BS.Sale_RateType,
       ISNULL(Round(SUM(BS.Sale_Nets),2),0),
       SUM(BS.NumberOfCalls),
       BS.Sale_Currency,
       @UserID,
       BS.MonthNumber,
       BS.YearNumber
FROM   #TempBillingStats BS 
GROUP BY
BS.MonthNumber,
       BS.YearNumber,
       BS.SaleZoneID,
               Round(BS.Sale_Rate,5),
       BS.Sale_RateType,
       BS.Sale_Currency


-----------------------------------------------------------

SELECT @Duration = SUM(bid.Duration),
       @NumberOfCalls = SUM(bid.NumberOfCalls)
FROM   Billing_Invoice_Details bid WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice))
WHERE  bid.InvoiceID = @InvoiceID

SELECT @Amount = ISNULL(Round(SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)),2),0) 
FROM #TempBillingStats BS
LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = DATEADD(day,0,datediff(day,0, BS.CallDate))

-----------------------------------------------------------

UPDATE Billing_Invoice
SET    Duration = ISNULL(@Duration,0),
       Amount = ISNULL(@Amount,0), 
       NumberOfCalls = ISNULL(@NumberOfCalls,0)
WHERE  InvoiceID = @InvoiceID

-- calculating the billing invoice costs (Gouped by the Out Carrier 'Supplier') 
INSERT INTO Billing_Invoice_Costs
(
    InvoiceID, SupplierID, Destination, FromDate, TillDate, Duration, Rate, 
    RateType, Amount, NumberOfCalls, CurrencyID,UserID
)      
SELECT 
       @InvoiceID,
       BS.SupplierID,
       BS.CostZoneID,
       MIN(BS.CallDate),
       MAX(BS.CallDate),
       SUM(BS.SaleDuration)/60.0,
       Round(BS.Sale_Rate,5),
       BS.Sale_RateType,
       ISNULL(Round(SUM(BS.Sale_Nets),2),0),
       SUM(BS.NumberOfCalls),
       BS.Sale_Currency,
       @UserID
FROM   #TempBillingStats BS 
GROUP BY
       BS.SupplierID,
       BS.CostZoneID,
       Round(BS.Sale_Rate,5),
       BS.Sale_RateType,
       BS.Sale_Currency 
       
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
SET InvoiceNotes = @TimeZoneInfo  WHERE InvoiceID = @InvoiceID
  
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