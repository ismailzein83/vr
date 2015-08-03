
CREATE PROCEDURE [dbo].[bp_PrepaidCarrierTotal_ForBridge]
    @ShowCustomerTotal char(1) = 'Y',
	@ShowSupplierTotal char(1) = 'Y'
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @ExchangeRates TABLE(Currency VARCHAR(3),
								 Date SMALLDATETIME,
								 Rate FLOAT
								 PRIMARY KEY(Currency, Date))
    
    DECLARE @Days TABLE(Date SMALLDATETIME)
    INSERT INTO @Days SELECT DISTINCT pa.Date FROM dbo.PrepaidAmount pa
	
	DECLARE @count INT
	
	DECLARE @Min DATETIME;
	SET @Min = (SELECT MIN(date) FROM dbo.PrepaidAmount)
	
	DECLARE @Max DATETIME;
	SET @Max = (SELECT MAX(date) FROM dbo.PrepaidAmount)

	DECLARE @Temp DATETIME;
	SET @Temp = @Min
	
	WHILE @Temp <= @Max
		BEGIN 
			SET @count = (SELECT COUNT(*) FROM @Days d WHERE d.date = @Temp )
			IF @count > 0
			INSERT INTO @ExchangeRates 
			SELECT *
			FROM dbo.GetDailyExchangeRates(@Temp,@Temp) er
		SET @Temp = DateAdd(day,1,@Temp)
		END
		
	DECLARE @AccountType INT
	
	IF @ShowCustomerTotal = 'Y'
		BEGIN
	SET @AccountType = (SELECT Value FROM dbo.Enumerations WHERE Enumeration = 'TABS.AccountType' AND Name = 'Termination');
	WITH TB1 AS (
				 SELECT pa.CustomerID AS CustomerID,
						ca.NameSuffix as Suffix,
						cp.Name as CarrierName,
						pa.CustomerProfileID AS CustomerProfileID,
						--pa.Amount * (SELECT Rate FROM dbo.GetDailyExchangeRates(pa.Date,pa.Date) er WHERE er.Currency = CP.CurrencyID)/(SELECT Rate FROM dbo.GetDailyExchangeRates(pa.Date,pa.Date) er WHERE er.Currency = pa.CurrencyID) AS Amount,
						pa.Amount *  (CASE WHEN cp.CurrencyID = pa.CurrencyID THEN 1 ELSE (SELECT Rate FROM @ExchangeRates er WHERE er.Date = pa.Date AND er.Currency = cp.CurrencyID)/(SELECT Rate FROM @ExchangeRates er WHERE er.Date = pa.Date AND er.Currency = pa.CurrencyID)END) AS Amount,
						cp.CurrencyID AS CurrencyID,
						pa.Type AS TYPE,
						pa.Date AS Date,
					   (CASE WHEN pa.CustomerID IS NOT NULL THEN ca.CustomerCreditLimit ELSE cp.CustomerCreditLimit END) AS CreditLimit
				 FROM PrepaidAmount pa
				 LEFT JOIN CarrierAccount ca ON ca.CarrierAccountID = pa.CustomerID AND ca.AccountType != @AccountType
				 LEFT JOIN CarrierProfile cp ON cp.ProfileID = pa.CustomerProfileID OR cp.ProfileID = ca.ProfileID 
				 WHERE (pa.CustomerID IS NOT NULL OR pa.CustomerProfileID IS NOT NULL)
				    AND pa.Date >= (CASE WHEN pa.CustomerID IS NOT NULL THEN ca.CustomerActivateDate ELSE cp.CustomerActivateDate END)
					AND (
						(CASE WHEN pa.CustomerID IS NOT NULL THEN ca.CustomerDeactivateDate ELSE cp.CustomerDeactivateDate END) IS NULL
						OR   
						(CASE WHEN pa.CustomerID IS NOT NULL THEN ca.CustomerDeactivateDate ELSE cp.CustomerDeactivateDate END) > pa.Date
						)
				 )
	    , TB2 AS (
				 SELECT tb1.CustomerID AS CarrierID,
				 TB1.Suffix as Suffix,
				 TB1.CarrierName as CarrierName,
						tb1.CustomerProfileID AS ProfileID,
						SUM(tb1.Amount) AS Balance,
						tb1.CurrencyID AS Currency,
						SUM(tb1.Amount) AS Tolerance,
						SUM(CASE WHEN tb1.[Type] = 1 THEN tb1.Amount ELSE 0 END) AS Billing,
						SUM(CASE WHEN tb1.[Type] != 1 THEN tb1.Amount ELSE 0 END) AS Payment,
						tb1.CreditLimit AS CreditLimit
				 FROM TB1 tb1
				 GROUP BY	tb1.CustomerProfileID,
							tb1.CustomerID,
							tb1.CurrencyID,
							tb1.CreditLimit,tb1.CarrierName,tb1.Suffix
						)
	SELECT 
			CarrierID,
		   ProfileID,
		   Suffix,
		   CarrierName,
		   (CASE WHEN Balance IS NOT NULL THEN Balance ELSE 0 END) AS Balance,
		    Currency,
		   (CASE WHEN Tolerance IS NOT NULL THEN Tolerance ELSE 0 END) AS Tolerance,
		   (CASE WHEN Billing IS NOT NULL THEN Billing ELSE 0 END) AS Billing,
		   (CASE WHEN Payment IS NOT NULL THEN Payment ELSE 0 END)  AS Payment,
		   CreditLimit
	FROM TB2
	ORDER BY Tolerance
END
	IF @ShowSupplierTotal = 'Y'
	BEGIN
	SET @AccountType = (SELECT Value FROM dbo.Enumerations WHERE Enumeration = 'TABS.AccountType' AND Name = 'Client');
	WITH TB1 AS (
				 SELECT pa.SupplierID AS SupplierID,
						ca.NameSuffix Suffix,
						cp.Name CarrierName,
						pa.SupplierProfileID AS SupplierProfileID,
						pa.Amount *  (CASE WHEN cp.CurrencyID = pa.CurrencyID THEN 1 ELSE (SELECT Rate FROM @ExchangeRates er WHERE er.Date = pa.Date AND er.Currency = cp.CurrencyID)/(SELECT Rate FROM @ExchangeRates er WHERE er.Date = pa.Date AND er.Currency = pa.CurrencyID)END) AS Amount,
						cp.CurrencyID AS CurrencyID,
						pa.Type AS TYPE,
						pa.Date AS Date,
					   (CASE WHEN pa.SupplierID IS NOT NULL THEN ca.SupplierCreditLimit ELSE cp.SupplierCreditLimit END) AS CreditLimit
				 FROM PrepaidAmount pa
				 LEFT JOIN CarrierAccount ca ON ca.CarrierAccountID = pa.SupplierID AND ca.AccountType != @AccountType
				 LEFT JOIN CarrierProfile cp ON cp.ProfileID = pa.SupplierProfileID OR cp.ProfileID = ca.ProfileID 
				 WHERE (pa.SupplierID IS NOT NULL OR pa.SupplierProfileID IS NOT NULL)
				    AND pa.Date >= (CASE WHEN pa.SupplierID IS NOT NULL THEN ca.SupplierActivateDate ELSE cp.SupplierActivateDate END)
					AND (
						(CASE WHEN pa.SupplierID IS NOT NULL THEN ca.SupplierDeactivateDate ELSE cp.SupplierDeactivateDate END) IS NULL
						OR   
						(CASE WHEN pa.SupplierID IS NOT NULL THEN ca.SupplierDeactivateDate ELSE cp.SupplierDeactivateDate END) > pa.Date
						)
				 )
	    , TB2 AS (
				 SELECT tb1.SupplierID AS CarrierID,
						tb1.CarrierName,
						tb1.Suffix,
						tb1.SupplierProfileID AS ProfileID,
						SUM(tb1.Amount) AS Balance,
						tb1.CurrencyID AS Currency,
						SUM(tb1.Amount) AS Tolerance,
						SUM(CASE WHEN tb1.[Type] = 1 THEN tb1.Amount ELSE 0 END) AS Billing,
						SUM(CASE WHEN tb1.[Type] != 1 THEN tb1.Amount ELSE 0 END) AS Payment,
						tb1.CreditLimit AS CreditLimit
				 FROM TB1 tb1
				 GROUP BY	tb1.SupplierProfileID,
							tb1.SupplierID,
							tb1.CurrencyID,
							tb1.CreditLimit,
							tb1.CarrierName,
							tb1.Suffix
						)
						
	SELECT 
			CarrierID,
		    ProfileID,
		    CarrierName,
		    Suffix,
		   (CASE WHEN Balance IS NOT NULL THEN Balance ELSE 0 END) AS Balance,
		    Currency,
		   (CASE WHEN Tolerance IS NOT NULL THEN Tolerance ELSE 0 END) AS Tolerance,
		   (CASE WHEN Billing IS NOT NULL THEN Billing ELSE 0 END) AS Billing,
		   (CASE WHEN Payment IS NOT NULL THEN Payment ELSE 0 END)  AS Payment,
		   CreditLimit
	FROM TB2
	ORDER BY Tolerance 
	 END
END