

-- ======================================================================
-- Author: Mazen Ghool
-- Description: Updates the daily Prepaid Amount Billing information 
-- ======================================================================
CREATE PROCEDURE [dbo].[bp_PrepaidDailyTotalCarrierUpdate]
(
	@FromCallDate datetime = NULL,
    @ToCallDate datetime  = NULL,
    @CarrierType varchar(10) = NULL,
    @CarrierProfileID int = 0,
    @CarrierID varchar(5) = NULL
    
)
WITH RECOMPILE
AS
BEGIN
	
	SET @FromCallDate = ISNULL(@FromCallDate, getdate())
	SET @ToCallDate = ISNULL(@ToCallDate, getdate())
			
	SET @FromCallDate = dbo.DateOf(@FromCallDate)
	SET @ToCallDate = dbo.DateOf(@ToCallDate)

	DECLARE @PrepaidType int
	SELECT @PrepaidType = e.Value
	  FROM Enumerations e 
			WHERE e.Enumeration = 'TABS.PaymentType' 
			AND [Name] = 'Prepaid'

	DECLARE @BillingPrepaidAmountType int
	SELECT @BillingPrepaidAmountType = e.Value
	  FROM Enumerations e 
			WHERE e.Enumeration = 'TABS.AmountType' 
			AND [Name] = 'Billing'

	DECLARE @LastUpdate datetime
	SET @LastUpdate = getdate()
	
	SET NOCOUNT ON;

	-- Clean up Billing Amounts
	DELETE 
		FROM PrepaidAmount 
		WHERE 
			Date BETWEEN @FromCallDate AND @ToCallDate 
			AND [TYPE] = @BillingPrepaidAmountType
			AND (
					(
						(@CarrierType IS NULL OR @CarrierType = 'Customer')
					AND (@CarrierID IS NULL OR CustomerID = @CarrierID)
					AND (@CarrierProfileID = 0 OR CustomerProfileID = @CarrierProfileID)
					)
				OR 
					(
						(@CarrierType IS NULL OR @CarrierType = 'Supplier')
					AND (@CarrierID IS NULL OR SupplierID = @CarrierID)
					AND (@CarrierProfileID = 0 OR SupplierProfileID = @CarrierProfileID)
					)
				)
	--

	-- Re-Insert Customer Amounts
	IF (@CarrierType IS NULL OR @CarrierType = 'Customer')
	
	INSERT INTO PrepaidAmount
	   (
			  CustomerID,
			  SupplierID,
			  Date,
			  Amount,
			  CurrencyID,
			  [TYPE],
			  LastUpdate
		)
	SELECT     
			BS.CustomerID,
			NULL,
			BS.CallDate,
			-1 * SUM(BS.Sale_Nets) AS Sale_Nets,
			BS.Sale_Currency,
			@BillingPrepaidAmountType,
			@LastUpdate
	FROM       
			Billing_Stats as BS 
	WHERE 1=1
		AND BS.CallDate >= @FromCallDate 
		AND BS.CallDate <= @ToCallDate
		AND (@CarrierID IS NULL OR BS.CustomerID = @CarrierID)
		AND BS.CustomerID IN (SELECT CarrierAccountID FROM CarrierAccount WHERE CustomerPaymentType = @PrepaidType)
		AND BS.Sale_Currency IS NOT NULL
	GROUP BY   
		BS.CustomerID, 
		BS.CallDate,  
		BS.Sale_Currency
	
	
	-- Re-Insert Customer Amounts for Prepaid Profiles
	IF (@CarrierType IS NULL OR @CarrierType = 'Customer')
	INSERT INTO PrepaidAmount
	   (
			  CustomerProfileID,
			  SupplierProfileID,
			  Date,
			  Amount,
			  CurrencyID,
			  [TYPE],
			  LastUpdate
		)
	SELECT     
			ca.ProfileID, -- BS.CustomerID,
			NULL,
			BS.CallDate,
			-1 * SUM(BS.Sale_Nets) AS Sale_Nets,
			BS.Sale_Currency,
			@BillingPrepaidAmountType,
			@LastUpdate
	FROM       
			Billing_Stats as BS JOIN CarrierAccount ca ON BS.CustomerID = ca.CarrierAccountID 
	WHERE 1=1
		AND BS.CallDate >= @FromCallDate 
		AND BS.CallDate <= @ToCallDate
		AND (@CarrierProfileID = 0 OR ca.ProfileID = @CarrierProfileID)
		AND BS.CustomerID IN (SELECT CarrierAccountID FROM CarrierAccount cat, CarrierProfile cpt 
		                      WHERE cat.ProfileID = cpt.ProfileID 
									AND cpt.CustomerPaymentType = @PrepaidType)
		AND BS.Sale_Currency IS NOT NULL
	GROUP BY   
		ca.ProfileID, 
		BS.CallDate,  
		BS.Sale_Currency
	
	
	-- Re-Insert Supplier Amounts
	IF(@CarrierType IS NULL OR @CarrierType = 'Supplier')
	INSERT INTO PrepaidAmount
	   (
			  CustomerID,
			  SupplierID,
			  Date,
			  Amount,
			  CurrencyID,
			  [TYPE],
			  LastUpdate
		)
	SELECT     
			NULL,
			BS.SupplierID,
			BS.CallDate,
			SUM(BS.Cost_Nets) AS Cost_Nets,
			BS.Cost_Currency,
			@BillingPrepaidAmountType,
			@LastUpdate
	FROM       
			Billing_Stats as BS 
	WHERE 1=1
		AND BS.CallDate >= @FromCallDate 
		AND BS.CallDate <= @ToCallDate
		AND (@CarrierID IS NULL OR BS.SupplierID = @CarrierID)
		AND BS.SupplierID IN (SELECT CarrierAccountID FROM CarrierAccount WHERE SupplierPaymentType = @PrepaidType)
		AND BS.Cost_Currency IS NOT NULL
	GROUP BY   
		BS.SupplierID, 
		BS.CallDate,  
		BS.Cost_Currency
	
-- Re-Insert Supplier Amounts Profile
	IF (@CarrierType IS NULL OR @CarrierType = 'Supplier')
	INSERT INTO PrepaidAmount
	   (
			  CustomerProfileID,
			  SupplierProfileID,
			  Date,
			  Amount,
			  CurrencyID,
			  [TYPE],
			  LastUpdate
		)
	SELECT     
			NULL,
			ca.ProfileID,
			BS.CallDate,
			SUM(BS.Cost_Nets) AS Cost_Nets,
			BS.Cost_Currency,
			@BillingPrepaidAmountType,
			@LastUpdate
	FROM       
			Billing_Stats as BS JOIN CarrierAccount ca ON BS.CustomerID = ca.CarrierAccountID
	WHERE 1=1
		AND BS.CallDate >= @FromCallDate 
		AND BS.CallDate <= @ToCallDate
		AND (@CarrierProfileID = 0 OR ca.ProfileID = @CarrierProfileID)
		AND BS.SupplierID IN (SELECT CarrierAccountID FROM CarrierAccount cat,CarrierProfile cpt WHERE cat.ProfileID = cpt.ProfileID AND cpt.SupplierPaymentType = @PrepaidType)
		AND BS.Cost_Currency IS NOT NULL
	GROUP BY   
		ca.ProfileID, 
		BS.CallDate,  
		BS.Cost_Currency
END