
CREATE PROCEDURE [dbo].[bp_CreateSupplierInvoiceZeroShift]
(
	@SupplierID varchar(5),
	@TimeZoneInfo VARCHAR(MAX),
	@FromDate Datetime,
	@ToDate Datetime,
	@IssueDate Datetime,
	@DueDate DATETIME,
	@UserID INT,
	@invoiceID int output
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

SET @FromDate = DATEADD(day,0,datediff(day,0, @FromDate))
SET @ToDate = DATEADD(s,-1,DATEADD(day,1,datediff(day,0, @ToDate)))

-- building a temp table that contains all billing data requested 
--------------------------------------------------------------------------------------
DECLARE @Pivot SMALLDATETIME
SET @Pivot = @FromDate
SELECT top 0 * INTO #TempBillingStats 
FROM Billing_Stats bs WITH(NOLOCK) WHERE 1=2

WHILE @Pivot <= @ToDate
BEGIN
    --------------    
INSERT INTO  #TempBillingStats 
SELECT *   
FROM Billing_Stats bs  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
WHERE  BS.CallDate = @Pivot
	  AND  BS.SupplierID COLLATE SQL_Latin1_General_CP1256_CI_AS  = @SupplierID COLLATE SQL_Latin1_General_CP1256_CI_AS 
	  AND  BS.Cost_Currency IS NOT NULL 
    --------------
SET @Pivot = DATEADD(dd, 1, @Pivot)
END 


-- getting the currency exchange rate for the selected customer  
----------------------------------------------------------
SELECT @CurrencyID = CurrencyID
FROM   CarrierProfile  (NOLOCK)
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
       @Duration = SUM(SaleDuration)/60.0,
       @Amount = SUM(Sale_Nets),
       @NumberOfCalls =  COUNT(*)
FROM  #TempBillingStats

IF(@Duration IS NULL OR @Amount IS NULL) RETURN
	BEGIN TRANSACTION;
		-- saving the billing invoice 
		
	INSERT INTO Billing_Invoice
	(
		BeginDate, EndDate, IssueDate, DueDate, CustomerID, SupplierID, SerialNumber, 
		Duration, Amount, NumberOfCalls, CurrencyID, IsLocked, IsPaid, UserID
	)

	SELECT @FromDate,
		   @ToDate,
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
	SELECT @InvoiceID = @@IDENTITY 
	UPDATE Billing_Invoice
	SET    EndDate = dateadd(dd,-1,EndDate)
	WHERE  InvoiceID = @InvoiceID


	-- Getting the Billing Invoice Details 

	INSERT INTO Billing_Invoice_Details
	(
		InvoiceID, Destination, FromDate, TillDate, Duration, Rate, RateType, Amount, NumberOfCalls, CurrencyID, UserID
	)
	SELECT @InvoiceID,
		   CAST( BS.CostZoneID AS CHAR(100)) COLLATE SQL_Latin1_General_CP1256_CI_AS ,
		   MIN(BS.CallDate),
		   MAX(BS.CallDate),
		   SUM(BS.CostDuration)/60.0,
		   Round(BS.Cost_Rate,5),
		   CAST( BS.Cost_RateType  AS CHAR(100)) COLLATE SQL_Latin1_General_CP1256_CI_AS  ,
		   ISNULL(Round(SUM(BS.Cost_Nets),2),0),
		   SUM(BS.NumberOfCalls) ,
		   BS.Cost_Currency,
		   @UserID
	 FROM  #TempBillingStats BS
	GROUP BY
		   CAST( BS.CostZoneID AS CHAR(100)) COLLATE SQL_Latin1_General_CP1256_CI_AS ,
		   Round(BS.Cost_Rate,5),
		   CAST( BS.Cost_RateType  AS CHAR(100)) COLLATE SQL_Latin1_General_CP1256_CI_AS  ,
		   BS.Cost_Currency


-----------------------------------------------------------

SELECT @Duration = SUM(bid.Duration),
       @NumberOfCalls = SUM(bid.NumberOfCalls)
FROM   Billing_Invoice_Details bid WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice))
WHERE  bid.InvoiceID = @InvoiceID

SELECT @Amount = ISNULL(Round(SUM(BS.Cost_Nets / ISNULL(ERS.Rate, 1)),2),0) 
FROM #TempBillingStats BS
LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Cost_Currency AND ERS.Date = DATEADD(day,0,datediff(day,0, BS.CallDate))

-----------------------------------------------------------


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