-- ======================================================================
-- Description: Updates the daily Postpaid Amount Billing information 
-- ======================================================================
CREATE PROCEDURE [dbo].[bp_PostpaidDailyTotalUpdate]
(
	@FromCallDate datetime = NULL,
    @ToCallDate datetime  = NULL
)
WITH RECOMPILE
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @PostpaidAmount TABLE
	(
		[SupplierID] [varchar](10) NULL,
		[CustomerID] [varchar](10) NULL,
		[Amount] [numeric](13, 5) NULL,
		[CurrencyID] [varchar](3) NULL,
		[Date] [datetime] NULL,
		[Type] [smallint] NULL,
		[UserID] [int] NULL,
		[Tag] [varchar](255) NULL,
		[LastUpdate] [datetime] NULL,
		[ReferenceNumber] [varchar](250) NULL,
		[Note] [varchar](250) NULL,
		[CustomerProfileID] [smallint] NULL,
		[SupplierProfileID] [smallint] NULL	
	)
	
	INSERT INTO @PostpaidAmount (UserID) VALUES (-1)
	
	SET @FromCallDate = ISNULL(@FromCallDate, getdate())
	SET @ToCallDate = ISNULL(@ToCallDate, getdate())
	
	SET @FromCallDate = dbo.DateOf(@FromCallDate)
	SET @ToCallDate = dbo.DateOf(@ToCallDate)

	DECLARE @PostpaidType int
	SELECT @PostpaidType = e.Value
	  FROM Enumerations e 
			WHERE e.Enumeration = 'TABS.PaymentType' 
			AND [Name] = 'Postpaid'

	DECLARE @BillingPostpaidAmountType int
	SELECT @BillingPostpaidAmountType = e.Value
	  FROM Enumerations e 
			WHERE e.Enumeration = 'TABS.AmountType' 
			AND [Name] = 'Billing'

	DECLARE @LastUpdate datetime
	SET @LastUpdate = getdate()
	
	CREATE TABLE #ExchangeRates (
			CurrencyIn VARCHAR(3) COLLATE SQL_Latin1_General_CP1256_CI_AS,
			CurrencyProfile VARCHAR(3) COLLATE SQL_Latin1_General_CP1256_CI_AS,
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(CurrencyIn,CurrencyProfile, Date)
	)
	
	INSERT INTO #ExchangeRates 
	SELECT gder1.Currency AS CurrencyIn,
		   gder2.Currency AS CurrencyProfile,
		   gder1.Date AS Date, 
		   gder1.Rate / gder2.Rate AS Rate
		FROM   dbo.GetDailyExchangeRates(@FromCallDate, @ToCallDate) gder1
		JOIN dbo.GetDailyExchangeRates(@FromCallDate, @ToCallDate) gder2 ON  gder1.Date = gder2.Date

	-- Clean up Billing Amounts
	DELETE FROM PostpaidAmount WHERE 
		Date BETWEEN @FromCallDate and @ToCallDate AND [TYPE] = @BillingPostpaidAmountType
	--

	-- Re-Insert Customer Amounts
	INSERT INTO @PostpaidAmount
	   (
			  CustomerID,
			  Date,
			  Amount,
			  CurrencyID,
			  [TYPE],
			  LastUpdate
		)
	SELECT     
			BS.CustomerID,
			BS.CallDate,
			-1 * SUM(BS.Sale_Nets)/er.Rate AS Sale_Nets,
			cp.CurrencyID,
			@BillingPostpaidAmountType,
			@LastUpdate
	FROM       
			Billing_Stats as BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
			JOIN CarrierAccount ca WITH (NOLOCK) ON BS.CustomerID = ca.CarrierAccountID AND ca.CustomerPaymentType = @PostpaidType
			JOIN CarrierProfile cp WITH (NOLOCK) ON cp.ProfileID = ca.ProfileID
			JOIN #ExchangeRates er WITH (NOLOCK) ON er.CurrencyIn = BS.Sale_Currency AND er.CurrencyProfile = cp.CurrencyID AND er.Date = BS.CallDate
	WHERE 1=1
		AND BS.CallDate >= @FromCallDate
		AND BS.CallDate <= @ToCallDate
		AND BS.CallDate >= ca.CustomerActivateDate AND (ca.CustomerDeactivateDate IS NULL OR ca.CustomerDeactivateDate > BS.CallDate)
		--AND BS.CustomerID IN (SELECT CarrierAccountID FROM CarrierAccount WHERE CustomerPaymentType = @PostpaidType)
		AND BS.Sale_Currency IS NOT NULL
	GROUP BY   
		BS.CustomerID, 
		BS.CallDate,  
		cp.CurrencyID,
		er.Rate

	-- Re-Insert Customer Amounts for Postpaid Profiles
	INSERT INTO @PostpaidAmount
	   (
			  CustomerProfileID,
			  Date,
			  Amount,
			  CurrencyID,
			  [TYPE],
			  LastUpdate
		)
	SELECT     
			ca.ProfileID, 
			BS.CallDate,
			-1 * SUM(BS.Sale_Nets)/er.Rate AS Sale_Nets,
			cp.CurrencyID,
			@BillingPostpaidAmountType,
			@LastUpdate
	FROM       
			Billing_Stats as BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
			JOIN CarrierAccount ca WITH (NOLOCK) ON BS.CustomerID = ca.CarrierAccountID 
			JOIN CarrierProfile cp WITH (NOLOCK) ON cp.ProfileID = ca.ProfileID AND cp.CustomerPaymentType = @PostpaidType
			JOIN #ExchangeRates er WITH (NOLOCK) ON er.CurrencyIn = BS.Sale_Currency AND er.CurrencyProfile = cp.CurrencyID AND er.Date = BS.CallDate
	WHERE 1=1
		AND BS.CallDate >= @FromCallDate 
		AND BS.CallDate <= @ToCallDate
		AND BS.CallDate >= cp.CustomerActivateDate AND (cp.CustomerDeactivateDate IS NULL OR cp.CustomerDeactivateDate > BS.CallDate)
		--AND BS.CustomerID IN (SELECT CarrierAccountID FROM CarrierAccount cat, CarrierProfile cpt WHERE cat.ProfileID = cpt.ProfileID AND cpt.CustomerPaymentType = @PostpaidType)
		AND BS.Sale_Currency IS NOT NULL
	GROUP BY   
		ca.ProfileID, 
		BS.CallDate,  
		cp.CurrencyID,
		er.Rate

	-- Re-Insert Supplier Amounts
	INSERT INTO @PostpaidAmount
	   (
			  SupplierID,
			  Date,
			  Amount,
			  CurrencyID,
			  [TYPE],
			  LastUpdate
		)
	SELECT     
			BS.SupplierID,
			BS.CallDate,
			SUM(BS.Cost_Nets)/er.Rate AS Cost_Nets,
			cp.CurrencyID,
			@BillingPostpaidAmountType,
			@LastUpdate
	FROM       
			Billing_Stats as BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
			JOIN CarrierAccount ca WITH (NOLOCK) ON BS.SupplierID = ca.CarrierAccountID AND ca.SupplierPaymentType = @PostpaidType
			JOIN CarrierProfile cp WITH (NOLOCK) ON cp.ProfileID = ca.ProfileID
			JOIN #ExchangeRates er WITH (NOLOCK) ON er.CurrencyIn = BS.Cost_Currency AND er.CurrencyProfile = cp.CurrencyID AND er.Date = BS.CallDate
	WHERE 1=1
		AND BS.CallDate >= @FromCallDate 
		AND BS.CallDate <= @ToCallDate
		AND BS.CallDate >= ca.SupplierActivateDate AND (ca.SupplierDeactivateDate IS NULL OR ca.SupplierDeactivateDate > BS.CallDate)
		--AND BS.SupplierID IN (SELECT CarrierAccountID FROM CarrierAccount WHERE SupplierPaymentType = @PostpaidType)
		AND BS.Cost_Currency IS NOT NULL
	GROUP BY   
		BS.SupplierID, 
		BS.CallDate,  
		cp.CurrencyID,
		er.Rate

-- Re-Insert Supplier Amounts Profile
	INSERT INTO @PostpaidAmount
	   (
			  SupplierProfileID,
			  Date,
			  Amount,
			  CurrencyID,
			  [TYPE],
			  LastUpdate
		)
	SELECT     
			ca.ProfileID,
			BS.CallDate,
			SUM(BS.Cost_Nets)/er.Rate AS Cost_Nets,
			cp.CurrencyID,
			@BillingPostpaidAmountType,
			@LastUpdate
	FROM       
			Billing_Stats as BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
			JOIN CarrierAccount ca WITH (NOLOCK) ON BS.SupplierID = ca.CarrierAccountID
			JOIN CarrierProfile cp WITH (NOLOCK) ON ca.ProfileID = cp.ProfileID AND cp.SupplierPaymentType = @PostpaidType
			JOIN #ExchangeRates er WITH (NOLOCK) ON er.CurrencyIn = BS.Cost_Currency AND er.CurrencyProfile = cp.CurrencyID AND er.Date = BS.CallDate
	WHERE 1=1
		AND BS.CallDate >= @FromCallDate 
		AND BS.CallDate <= @ToCallDate
		AND BS.CallDate >= cp.SupplierActivateDate AND (cp.SupplierDeactivateDate IS NULL OR cp.SupplierDeactivateDate > BS.CallDate)
		--AND BS.SupplierID IN (SELECT CarrierAccountID FROM CarrierAccount cat,CarrierProfile cpt WHERE cat.ProfileID = cpt.ProfileID AND cpt.SupplierPaymentType = @PostpaidType)
		AND BS.Cost_Currency IS NOT NULL
	GROUP BY   
		ca.ProfileID, 
		BS.CallDate,  
		cp.CurrencyID,
		er.Rate
		
	INSERT INTO PostpaidAmount 
		(
		[SupplierID],
		[CustomerID],
		[Amount],
		[CurrencyID],
		[Date],
		[Type],
		[UserID],
		[Tag],
		[LastUpdate],
		[ReferenceNumber],
		[Note],
		[CustomerProfileID],
		[SupplierProfileID] 
		)
	SELECT 		
			[SupplierID],
			[CustomerID],
			[Amount],
			[CurrencyID],
			[Date],
			[Type],
			[UserID],
			[Tag],
			[LastUpdate],
			[ReferenceNumber],
			[Note],
			[CustomerProfileID],
			[SupplierProfileID] 
	FROM @PostpaidAmount WHERE UserID IS NULL

END