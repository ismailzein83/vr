
CREATE PROCEDURE [dbo].[bp_CreateSupplierInvoice]
(
	@SupplierID varchar(5),
	@GMTShifttime int,
	@TimeZoneInfo VARCHAR(MAX),
	@FromDate Datetime,
	@ToDate Datetime,
	@IssueDate Datetime,
	@DueDate DATETIME,
	@UserID INT,
	@InvoiceID int output
)
AS
BEGIN
	
	SET NOCOUNT ON 

	-- Creating the Invoice 	
	DECLARE @Amount float 
	DECLARE @Duration NUMERIC(19,6)  
	DECLARE @NumberOfCalls INT 
	DECLARE @CurrencyID varchar(3) 
	DECLARE @Serial int
	DECLARE @CurrencyExchangeRate decimal 
DECLARE @From SMALLDATETIME
SET @From = @FromDate

DECLARE @To SMALLDATETIME
SET @To = @ToDate
	--SET @FromDate = CAST(
	--(
	--STR( YEAR( @FromDate ) ) + '-' +
	--STR( MONTH( @FromDate ) ) + '-' +
	--STR( DAY( @FromDate ) )
	--)
	--AS DATETIME
	--)

	--SET @ToDate = CAST(
	--(
	--STR( YEAR( @ToDate ) ) + '-' +
	--STR( MONTH(@ToDate ) ) + '-' +
	--STR( DAY( @ToDate ) ) 
	--)
	--AS DATETIME
	--)

-- Apply the Time Shift 
SET @FromDate = dateadd(mi,-@GMTShifttime,@FromDate);
SET @ToDate = dateadd(mi,-@GMTShifttime,@ToDate);

-- building a temp table that contains all billing data requested 
--------------------------------------------------------------------------------------

DECLARE @Pivot SMALLDATETIME
SET @Pivot = @FromDate

SELECT 
bcm.Attempt, bcm.Connect,
       bcm.CustomerID, bcm.OurZoneID,
       bcm.SupplierID, bcm.SupplierZoneID, bcc.ZoneID, bcc.Net, bcc.CurrencyID,
       bcc.RateValue, bcc.RateType, bcc.DurationInSeconds 
 INTO #TempBillingCDR 
  FROM  Billing_CDR_Main bcm WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt))
	   LEFT JOIN Billing_CDR_Cost bcc (NOLOCK) ON  bcc.ID = bcm.ID
 WHERE  1=2--bcm.Attempt  = '1900-01-01'
  
WHILE @Pivot <= @ToDate
BEGIN
 ----------------------
 DECLARE @BeginDate DATETIME
 DECLARE @EndDate DATETIME
 SET @BeginDate = @Pivot

 SET @EndDate = DATEADD(dd, 1, @Pivot)

INSERT  INTO  #TempBillingCDR  
SELECT bcm.Attempt, bcm.Connect,
       bcm.CustomerID, bcm.OurZoneID,
       bcm.SupplierID, bcm.SupplierZoneID, bcc.ZoneID, bcc.Net, bcc.CurrencyID,
       bcc.RateValue, bcc.RateType, bcc.DurationInSeconds
 FROM  Billing_CDR_Main bcm WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt,IX_Billing_CDR_Main_Supplier))
	   LEFT JOIN Billing_CDR_Cost bcc (NOLOCK) ON  bcc.ID = bcm.ID
 WHERE  bcm.Attempt >= @BeginDate
	  AND  bcm.Attempt < @EndDate
	  AND  bcm.SupplierID = @SupplierID  
	  AND  bcc.CurrencyID IS NOT NULL 
  ------------------------------ 
    SET @Pivot = DATEADD(dd, 1, @Pivot)
END 


UPDATE #TempBillingCDR
SET Attempt =  dateadd(mi,@GMTShifttime,Attempt),
    Connect =  dateadd(mi,@GMTShifttime,Connect)

-- Apply the Time Shift 
SET @FromDate = dateadd(mi,@GMTShifttime,@FromDate);
SET @ToDate = dateadd(mi,@GMTShifttime,@ToDate);
-- getting the currency exchange rate for the selected customer  
----------------------------------------------------------
SELECT @CurrencyID = CurrencyID
FROM   CarrierProfile (NOLOCK)
WHERE  ProfileID = ( 
           SELECT ProfileID
           FROM   CarrierAccount (NOLOCK)
           WHERE  CarrierAccountID = @SupplierID
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
	-- calculating the total duration and amount for the invoice to be created 
	SET @Serial = '1' 
SELECT 
       @Duration = SUM(DurationInSeconds)/60.0,
       @Amount = SUM(Net),
       @NumberOfCalls =  COUNT(*)
FROM  #TempBillingCDR

IF(@Duration IS NULL OR @Amount IS NULL) RETURN
	BEGIN TRANSACTION;
		-- saving the billing invoice 
		
	INSERT INTO Billing_Invoice
	(
		BeginDate, EndDate, IssueDate, DueDate, CustomerID, SupplierID, SerialNumber, 
		Duration, Amount, NumberOfCalls, CurrencyID, IsLocked, IsPaid, UserID
	)

	SELECT @From,
		   @To,
		   @IssueDate,
		   @DueDate,
		   'SYS',
		   @SupplierID,
		   @Serial,
		   ISNULL(@Duration,0)/60.0,
		   ISNULL(@Amount,0),
		   ISNULL(@NumberOfCalls,0),
		   @CurrencyID,
		   'N',
		   'N',
			@UserID 
	       
		   -- Getting the Invoice ID saved 
   SELECT @InvoiceID = SCOPE_IDENTITY()

	-- Getting the Billing Invoice Details 

	INSERT INTO Billing_Invoice_Details
	(
		InvoiceID, Destination, FromDate, TillDate, Duration, Rate, RateType, Amount, NumberOfCalls, CurrencyID, UserID
	)
	SELECT 
      @InvoiceID,
       B.ZoneID,
       MIN(B.Attempt),
       MAX(B.Attempt),
       SUM(B.DurationInSeconds)/60.0,
       Round(B.RateValue,5) ,
       B.RateType,
       ISNULL(Round(SUM(B.Net),2),0),
       COUNT(*),
       B.CurrencyID,
       @UserID
    FROM    #TempBillingCDR B
	GROUP BY
		   B.ZoneID,
		   Round(B.RateValue,5),
		   B.RateType,
		   B.CurrencyID

SELECT @Duration = SUM(bid.Duration),
       @NumberOfCalls = SUM(bid.NumberOfCalls)
FROM   Billing_Invoice_Details bid WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice))
WHERE  bid.InvoiceID = @InvoiceID

SELECT @Amount =   ISNULL(Round(SUM(B.Net / ISNULL(ERS.Rate, 1)),2),0) 
FROM #TempBillingCDR B
LEFT JOIN @ExchangeRates ERS ON ERS.Currency = B.CurrencyID AND ERS.Date = DATEADD(day,0,datediff(day,0, B.Attempt))


	UPDATE Billing_Invoice
	SET    Duration = ISNULL(@Duration,0),
		   Amount = ISNULL(@Amount,0) ,
		   NumberOfCalls = ISNULL(@NumberOfCalls,0)
	WHERE  InvoiceID = @InvoiceID

	-- updating the ZoneName 
	UPDATE Billing_Invoice_Details
	SET    Destination = Zone.Name
	FROM   Billing_Invoice_Details WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice)),
		   Zone WITH(NOLOCK)
	WHERE  Billing_Invoice_Details.Destination = Zone.ZoneID
	  AND  Billing_Invoice_Details.InvoiceID = @InvoiceID



	UPDATE Billing_Invoice
	SET InvoiceNotes = @TimeZoneInfo WHERE InvoiceID = @InvoiceID
	
	select @InvoiceID
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

END