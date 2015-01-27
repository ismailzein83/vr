-- ======================================================================
-- Author: Mohammad El-Shab
-- Description: 
-- ======================================================================
CREATE PROCEDURE [dbo].[bp_PostpaidCarrierTotal_FiancialMonitor]
    @ShowCustomerTotal char(1) = 'Y',
	@ShowSupplierTotal char(1) = 'Y',
	@IsNettingEnabled CHAR(1) = 'N',
	@FromDate SMALLDATETIME = NULL,
	@ToDate SMALLDATETIME = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @ExchangeRates TABLE(Currency VARCHAR(3),
								 Date SMALLDATETIME,
								 Rate FLOAT
								 PRIMARY KEY(Currency, Date))
    
    DECLARE @Days TABLE(Date SMALLDATETIME)
    INSERT INTO @Days SELECT DISTINCT pa.Date FROM dbo.PostpaidAmount pa
	
	DECLARE @count INT
	
	DECLARE @Min DATETIME;
	SET @Min = (SELECT MIN(date) FROM dbo.PostpaidAmount)
	
	DECLARE @Max DATETIME;
	SET @Max = (SELECT MAX(date) FROM dbo.PostpaidAmount)

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
	
	IF @IsNettingEnabled = 'N'
	
	BEGIN
		DECLARE @BillingType SMALLINT
		SELECT @BillingType = e.[Value] FROM Enumerations e WHERE e.Enumeration = 'TABS.AmountType' AND e.[Name] = 'Billing'
		
		IF @ShowCustomerTotal = 'Y'
		BEGIN
		WITH TB1 AS (
			 SELECT pa.CustomerID AS CustomerID,
					pa.CustomerProfileID AS CustomerProfileID,
					--pa.Amount * (SELECT Rate FROM dbo.GetDailyExchangeRates(pa.Date,pa.Date) er WHERE er.Currency = CP.CurrencyID)/(SELECT Rate FROM dbo.GetDailyExchangeRates(pa.Date,pa.Date) er WHERE er.Currency = pa.CurrencyID) AS Amount,
					pa.Amount *  (CASE WHEN cp.CurrencyID = pa.CurrencyID THEN 1 ELSE (SELECT Rate FROM @ExchangeRates er WHERE er.Date = pa.Date AND er.Currency = cp.CurrencyID)/(SELECT Rate FROM @ExchangeRates er WHERE er.Date = pa.Date AND er.Currency = pa.CurrencyID)END) AS Amount,
					cp.CurrencyID AS CurrencyID,
					pa.Type AS TYPE,
					pa.Date AS Date,
				   (CASE WHEN pa.CustomerID IS NOT NULL THEN ca.CustomerCreditLimit ELSE cp.CustomerCreditLimit END) AS CreditLimit
			 FROM dbo.PostpaidAmount pa
			 LEFT JOIN CarrierAccount ca ON ca.CarrierAccountID = pa.CustomerID
			 LEFT JOIN CarrierProfile cp ON cp.ProfileID = pa.CustomerProfileID OR cp.ProfileID = ca.ProfileID 
			 WHERE (pa.CustomerID IS NOT NULL OR pa.CustomerProfileID IS NOT NULL)
			    AND (cp.IsNettingEnabled = @IsNettingEnabled OR ca.IsNettingEnabled = @IsNettingEnabled)
			    AND pa.Date >= (CASE WHEN pa.CustomerID IS NOT NULL THEN ca.CustomerActivateDate ELSE cp.CustomerActivateDate END)
				AND (
					(CASE WHEN pa.CustomerID IS NOT NULL THEN ca.CustomerDeactivateDate ELSE cp.CustomerDeactivateDate END) IS NULL
					OR   
					(CASE WHEN pa.CustomerID IS NOT NULL THEN ca.CustomerDeactivateDate ELSE cp.CustomerDeactivateDate END) > pa.Date
					)
			 )
		     , TB2 AS (
			 SELECT tb1.CustomerID AS CarrierID,
					tb1.CustomerProfileID AS ProfileID,
					SUM(tb1.Amount) AS Balance,
					tb1.CurrencyID AS Currency,
					--(case WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END) AS CreditLimit,
					--ABS(ISNULL((CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END),0)) - ABS(SUM(PA.Amount)) AS Tolerance Old Fadi
					tb1.CreditLimit + SUM(tb1.Amount) AS Tolerance,
					SUM(CASE WHEN tb1.[Type] = @BillingType THEN tb1.Amount ELSE 0 END) AS Billing,
					SUM(CASE WHEN tb1.[Type] != @BillingType THEN tb1.Amount ELSE 0 END) AS Payment,
					tb1.CreditLimit AS CreditLimit
			 FROM TB1 tb1
			 GROUP BY	tb1.CustomerProfileID,
						tb1.CustomerID,
						tb1.CurrencyID,
						tb1.CreditLimit
					)
			SELECT CarrierID AS CarrierID,
				   ProfileID AS ProfileID,
				   CONVERT(DECIMAL(10,2),(CASE WHEN Balance IS NOT NULL THEN Balance ELSE 0 END)) AS Balance,
					Currency,
					CreditLimit AS CustomerCreditLimit,
				   CONVERT(DECIMAL(10,2),(CASE WHEN Tolerance IS NOT NULL THEN Tolerance ELSE 0 END)) AS Tolerance,
				   CONVERT(DECIMAL(10,2),(CASE WHEN Billing IS NOT NULL THEN Billing ELSE 0 END)) AS BillingAsCustomer,
				   CONVERT(DECIMAL(10,2),(CASE WHEN Payment IS NOT NULL THEN Payment ELSE 0 END))  AS PaymentAsCustomer
			FROM TB2
			ORDER BY Tolerance
		END
		IF @ShowSupplierTotal = 'Y'
		BEGIN
			WITH TB1 AS (
			SELECT pa.SupplierID AS SupplierID,
					pa.SupplierProfileID AS SupplierProfileID,
					--pa.Amount * (SELECT Rate FROM dbo.GetDailyExchangeRates(pa.Date,pa.Date) er WHERE er.Currency = CP.CurrencyID)/(SELECT Rate FROM dbo.GetDailyExchangeRates(pa.Date,pa.Date) er WHERE er.Currency = pa.CurrencyID) AS Amount,
					pa.Amount *  (CASE WHEN cp.CurrencyID = pa.CurrencyID THEN 1 ELSE (SELECT Rate FROM @ExchangeRates er WHERE er.Date = pa.Date AND er.Currency = cp.CurrencyID)/(SELECT Rate FROM @ExchangeRates er WHERE er.Date = pa.Date AND er.Currency = pa.CurrencyID)END) AS Amount,
					cp.CurrencyID AS CurrencyID,
					pa.Type AS TYPE,
					pa.Date AS Date,
				   (CASE WHEN pa.SupplierID IS NOT NULL THEN ca.SupplierCreditLimit ELSE cp.SupplierCreditLimit END) AS CreditLimit
			 FROM PostpaidAmount pa
			 LEFT JOIN CarrierAccount ca ON ca.CarrierAccountID = pa.SupplierID
			 LEFT JOIN CarrierProfile cp ON cp.ProfileID = pa.SupplierProfileID OR cp.ProfileID = ca.ProfileID 
			 WHERE (pa.SupplierID IS NOT NULL OR pa.SupplierProfileID IS NOT NULL)
			    AND (cp.IsNettingEnabled = @IsNettingEnabled OR ca.IsNettingEnabled = @IsNettingEnabled)
			    AND pa.Date >= (CASE WHEN pa.SupplierID IS NOT NULL THEN ca.SupplierActivateDate ELSE cp.SupplierActivateDate END)
				AND (
					(CASE WHEN pa.SupplierID IS NOT NULL THEN ca.SupplierDeactivateDate ELSE cp.SupplierDeactivateDate END) IS NULL
					OR   
					(CASE WHEN pa.SupplierID IS NOT NULL THEN ca.SupplierDeactivateDate ELSE cp.SupplierDeactivateDate END) > pa.Date
					)
			 )
			, TB2 AS (
			 SELECT tb1.SupplierID AS CarrierID,
					tb1.SupplierProfileID AS ProfileID,
					SUM(tb1.Amount) AS Balance,
					tb1.CurrencyID AS Currency,
					--(case WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END) AS CreditLimit,
					--ABS(ISNULL((CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END),0)) - ABS(SUM(PA.Amount)) AS Tolerance Old Fadi
					tb1.CreditLimit - SUM(tb1.Amount) AS Tolerance,
					SUM(CASE WHEN tb1.[Type] = @BillingType THEN tb1.Amount ELSE 0 END) AS Billing,
					SUM(CASE WHEN tb1.[Type] != @BillingType THEN tb1.Amount ELSE 0 END) AS Payment,
					tb1.CreditLimit AS CreditLimit
			 FROM TB1 tb1
			 GROUP BY	tb1.SupplierProfileID,
						tb1.SupplierID,
						tb1.CurrencyID,
						tb1.CreditLimit
					)
		SELECT CarrierID AS CarrierID,
			   ProfileID AS ProfileID,
			   CONVERT(DECIMAL(10,2),(CASE WHEN Balance IS NOT NULL THEN Balance ELSE 0 END)) AS Balance,
				Currency,
				CreditLimit AS SupplierCreditLimit,
			   CONVERT(DECIMAL(10,2),(CASE WHEN Tolerance IS NOT NULL THEN Tolerance ELSE 0 END)) AS Tolerance,
			   CONVERT(DECIMAL(10,2),(CASE WHEN Billing IS NOT NULL THEN Billing ELSE 0 END)) AS BillingAsSupplier,
			   CONVERT(DECIMAL(10,2),(CASE WHEN Payment IS NOT NULL THEN Payment ELSE 0 END) ) AS PaymentAsSupplier
		FROM TB2
		ORDER BY Tolerance
	END
	END
	ELSE
		BEGIN
			DECLARE @Amounts TABLE (
					CarrierID VARCHAR(10) NULL, 
					ProfileID SMALLINT NULL, 
					Balance NUMERIC(13,5) NULL, 
					BillingAsCustomer NUMERIC(13,5) NULL,  
					BillingAsSupplier NUMERIC(13,5) NULL,
					Currency VARCHAR(3) NULL,
					CustomerCreditLimit INT NULL,
					SupplierCreditLimit INT NULL,
					Tolerance NUMERIC(13,5) NULL,
					PaymentAsCustomer NUMERIC(13,5) NULL, 
					PaymentAsSupplier NUMERIC(13,5) NULL
			)
			
			DECLARE @BillingAmountType SMALLINT
			SELECT @BillingAmountType = e.[Value] FROM Enumerations e WHERE e.Enumeration = 'TABS.AmountType' AND e.[Name] = 'Billing'
			DECLARE @PaymentAmountType SMALLINT
			SELECT @PaymentAmountType = e.[Value] FROM Enumerations e WHERE e.Enumeration = 'TABS.AmountType' AND e.[Name] = 'Payment'
			
			-- Customer Account
			;WITH TB1 AS (
			 SELECT pa.CustomerID AS CustomerID,
					--pa.Amount * (SELECT Rate FROM dbo.GetDailyExchangeRates(pa.Date,pa.Date) er WHERE er.Currency = CP.CurrencyID)/(SELECT Rate FROM dbo.GetDailyExchangeRates(pa.Date,pa.Date) er WHERE er.Currency = pa.CurrencyID) AS Amount,
					pa.Amount * (CASE WHEN cp.CurrencyID = pa.CurrencyID THEN 1 ELSE (SELECT Rate FROM @ExchangeRates er WHERE er.Date = pa.Date AND er.Currency = cp.CurrencyID)/(SELECT Rate FROM @ExchangeRates er WHERE er.Date = pa.Date AND er.Currency = pa.CurrencyID)END) AS Amount,
					cp.CurrencyID AS CurrencyID,
					pa.Type AS [TYPE],
					pa.Date AS [Date],
				    ca.CustomerCreditLimit AS CustomerCreditLimit,
					ca.SupplierCreditLimit AS SupplierCreditLimit
			 FROM dbo.PostpaidAmount pa
			 LEFT JOIN CarrierAccount ca ON ca.CarrierAccountID = pa.CustomerID
			 LEFT JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
			 WHERE pa.CustomerID = ca.CarrierAccountID AND ca.IsNettingEnabled = @IsNettingEnabled
			    AND pa.Date >= ca.CustomerActivateDate
				AND (ca.CustomerDeactivateDate IS NULL
					OR   
					 ca.CustomerDeactivateDate > pa.Date
					)
					AND pa.CustomerID IS NOT NULL
			 )
		     , TB2 AS (
			 SELECT tb1.CustomerID AS CustomerID,
					SUM(tb1.Amount) AS Balance,
					tb1.CurrencyID AS Currency,
					--(case WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END) AS CreditLimit,
					--ABS(ISNULL((CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END),0)) - ABS(SUM(PA.Amount)) AS Tolerance Old Fadi
					SUM(CASE WHEN tb1.[Type] = @BillingAmountType THEN tb1.Amount ELSE 0 END) AS BillingAsCustomer,
					SUM(CASE WHEN tb1.[Type] != @BillingAmountType THEN tb1.Amount ELSE 0 END) AS PaymentAsCustomer,
					tb1.CustomerCreditLimit AS CustomerCreditLimit,
					tb1.SupplierCreditLimit AS SupplierCreditLimit
			 FROM TB1 tb1
			 GROUP BY	tb1.CustomerID,
						tb1.CurrencyID,
						tb1.CustomerCreditLimit,
						tb1.SupplierCreditLimit
					)
			INSERT INTO @Amounts	
			SELECT CustomerID AS CarrierID,
				   NULL AS ProfileID,
				   (CASE WHEN Balance IS NOT NULL THEN Balance ELSE 0 END) AS Balance,
				   (CASE WHEN BillingAsCustomer IS NOT NULL THEN BillingAsCustomer ELSE 0 END) AS BillingAsCustomer,
				   0.0 AS BillingAsSupplier,
				   tb2.Currency,
				   tb2.CustomerCreditLimit,
				   tb2.SupplierCreditLimit,
				   0 AS Tolerance,
				   (CASE WHEN PaymentAsCustomer IS NOT NULL THEN PaymentAsCustomer ELSE 0 END)  AS PaymentAsCustomer,
				   0.0 AS PaymentAsSupplier
			FROM TB2
				
		    -- Supplier Account
			;WITH TB1 AS (
			SELECT pa.SupplierID AS SupplierID,
				--pa.Amount * (SELECT Rate FROM dbo.GetDailyExchangeRates(pa.Date,pa.Date) er WHERE er.Currency = CP.CurrencyID)/(SELECT Rate FROM dbo.GetDailyExchangeRates(pa.Date,pa.Date) er WHERE er.Currency = pa.CurrencyID) AS Amount,
				pa.Amount * (CASE WHEN cp.CurrencyID = pa.CurrencyID THEN 1 ELSE (SELECT Rate FROM @ExchangeRates er WHERE er.Date = pa.Date AND er.Currency = cp.CurrencyID)/(SELECT Rate FROM @ExchangeRates er WHERE er.Date = pa.Date AND er.Currency = pa.CurrencyID)END) AS Amount,
				cp.CurrencyID AS CurrencyID,
				pa.Type AS [TYPE],
				pa.Date AS [Date],
				ca.CustomerCreditLimit AS CustomerCreditLimit,
				ca.SupplierCreditLimit AS SupplierCreditLimit
			FROM dbo.PostpaidAmount pa
			LEFT JOIN CarrierAccount ca ON ca.CarrierAccountID = pa.SupplierID
			LEFT JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
			WHERE pa.SupplierID = ca.CarrierAccountID AND ca.IsNettingEnabled = @IsNettingEnabled
			AND pa.Date >= ca.SupplierActivateDate
			AND (ca.SupplierDeactivateDate IS NULL
				OR   
				 ca.SupplierDeactivateDate > pa.Date
				)
				AND pa.SupplierID IS NOT NULL
			)
			, TB2 AS (
			SELECT tb1.SupplierID AS SupplierID,
				SUM(tb1.Amount) AS Balance,
				tb1.CurrencyID AS Currency,
				--(case WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END) AS CreditLimit,
				--ABS(ISNULL((CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END),0)) - ABS(SUM(PA.Amount)) AS Tolerance Old Fadi
				SUM(CASE WHEN tb1.[Type] = @BillingAmountType THEN tb1.Amount ELSE 0 END) AS BillingAsSupplier,
				SUM(CASE WHEN tb1.[Type] != @BillingAmountType THEN tb1.Amount ELSE 0 END) AS PaymentAsSupplier,
				tb1.CustomerCreditLimit AS CustomerCreditLimit,
				tb1.SupplierCreditLimit AS SupplierCreditLimit
			FROM TB1 tb1
			GROUP BY	tb1.SupplierID,
					tb1.CurrencyID,
					tb1.CustomerCreditLimit,
					tb1.SupplierCreditLimit
				)
			INSERT INTO @Amounts	
			SELECT SupplierID AS CarrierID,
			   NULL AS ProfileID,
			   (CASE WHEN Balance IS NOT NULL THEN Balance ELSE 0 END) AS Balance,
			   0.0 AS BillingAsCustomer,
			   (CASE WHEN BillingAsSupplier IS NOT NULL THEN BillingAsSupplier ELSE 0 END) AS BillingAsSupplier,
			   tb2.Currency,
			   tb2.CustomerCreditLimit,
			   tb2.SupplierCreditLimit,
			   0 AS Tolerance,
			   0.0 AS PaymentAsCustomer,
			   (CASE WHEN PaymentAsSupplier IS NOT NULL THEN PaymentAsSupplier ELSE 0 END)  AS PaymentAsSupplier
			FROM TB2
			
			-- Customer Profile
			;WITH TB1 AS (
			SELECT pa.CustomerProfileID AS CustomerProfileID,
				--pa.Amount * (SELECT Rate FROM dbo.GetDailyExchangeRates(pa.Date,pa.Date) er WHERE er.Currency = CP.CurrencyID)/(SELECT Rate FROM dbo.GetDailyExchangeRates(pa.Date,pa.Date) er WHERE er.Currency = pa.CurrencyID) AS Amount,
				pa.Amount * (CASE WHEN cp.CurrencyID = pa.CurrencyID THEN 1 ELSE (SELECT Rate FROM @ExchangeRates er WHERE er.Date = pa.Date AND er.Currency = cp.CurrencyID)/(SELECT Rate FROM @ExchangeRates er WHERE er.Date = pa.Date AND er.Currency = pa.CurrencyID)END) AS Amount,
				cp.CurrencyID AS CurrencyID,
				pa.Type AS [TYPE],
				pa.Date AS [Date],
				cp.CustomerCreditLimit AS CustomerCreditLimit,
				cp.SupplierCreditLimit AS SupplierCreditLimit
			FROM dbo.PostpaidAmount pa
			LEFT JOIN CarrierProfile cp ON cp.ProfileID = pa.CustomerProfileID
			WHERE pa.CustomerProfileID = cp.ProfileID AND cp.IsNettingEnabled = @IsNettingEnabled 
			AND pa.Date >= cp.CustomerActivateDate 
			AND (cp.CustomerDeactivateDate IS NULL OR cp.CustomerDeactivateDate > pa.Date)  
			AND pa.CustomerProfileID IS NOT NULL
			)
			, TB2 AS (
			SELECT tb1.CustomerProfileID AS CustomerProfileID,
				SUM(tb1.Amount) AS Balance,
				tb1.CurrencyID AS Currency,
				--(case WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END) AS CreditLimit,
				--ABS(ISNULL((CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END),0)) - ABS(SUM(PA.Amount)) AS Tolerance Old Fadi
				SUM(CASE WHEN tb1.[Type] = @BillingAmountType THEN tb1.Amount ELSE 0 END) AS BillingAsCustomer,
				SUM(CASE WHEN tb1.[Type] != @BillingAmountType THEN tb1.Amount ELSE 0 END) AS PaymentAsCustomer,
				tb1.CustomerCreditLimit AS CustomerCreditLimit,
				tb1.SupplierCreditLimit AS SupplierCreditLimit
			FROM TB1 tb1
			GROUP BY	tb1.CustomerProfileID,
					tb1.CurrencyID,
					tb1.CustomerCreditLimit,
					tb1.SupplierCreditLimit
				)
			INSERT INTO @Amounts	
			SELECT NULL AS CarrierID,
			   CustomerProfileID AS ProfileID,
			   (CASE WHEN Balance IS NOT NULL THEN Balance ELSE 0 END) AS Balance,
			   (CASE WHEN BillingAsCustomer IS NOT NULL THEN BillingAsCustomer ELSE 0 END) AS BillingAsCustomer,
			   0.0 AS BillingAsSupplier,
			   tb2.Currency,
			   tb2.CustomerCreditLimit,
			   tb2.SupplierCreditLimit,
			   0 AS Tolerance,
			   (CASE WHEN PaymentAsCustomer IS NOT NULL THEN PaymentAsCustomer ELSE 0 END)  AS PaymentAsCustomer,
			   0.0 AS PaymentAsSupplier
			FROM TB2
			
			-- Supplier Profile
			;WITH TB1 AS (
			SELECT pa.SupplierProfileID AS SupplierProfileID,
				--pa.Amount * (SELECT Rate FROM dbo.GetDailyExchangeRates(pa.Date,pa.Date) er WHERE er.Currency = CP.CurrencyID)/(SELECT Rate FROM dbo.GetDailyExchangeRates(pa.Date,pa.Date) er WHERE er.Currency = pa.CurrencyID) AS Amount,
				pa.Amount * (CASE WHEN cp.CurrencyID = pa.CurrencyID THEN 1 ELSE (SELECT Rate FROM @ExchangeRates er WHERE er.Date = pa.Date AND er.Currency = cp.CurrencyID)/(SELECT Rate FROM @ExchangeRates er WHERE er.Date = pa.Date AND er.Currency = pa.CurrencyID)END) AS Amount,
				cp.CurrencyID AS CurrencyID,
				pa.Type AS [TYPE],
				pa.Date AS [Date],
				cp.CustomerCreditLimit AS CustomerCreditLimit,
				cp.SupplierCreditLimit AS SupplierCreditLimit
			FROM dbo.PostpaidAmount pa
			LEFT JOIN CarrierProfile cp ON cp.ProfileID = pa.SupplierProfileID
			WHERE pa.SupplierProfileID = cp.ProfileID AND cp.IsNettingEnabled = @IsNettingEnabled
			AND pa.Date >= cp.SupplierActivateDate 
			AND (cp.SupplierDeactivateDate IS NULL OR cp.SupplierDeactivateDate > pa.Date)  
			AND pa.SupplierProfileID IS NOT NULL
			)
			, TB2 AS (
			SELECT tb1.SupplierProfileID AS SupplierProfileID,
				SUM(tb1.Amount) AS Balance,
				tb1.CurrencyID AS Currency,
				--(case WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END) AS CreditLimit,
				--ABS(ISNULL((CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END),0)) - ABS(SUM(PA.Amount)) AS Tolerance Old Fadi
				SUM(CASE WHEN tb1.[Type] = @BillingAmountType THEN tb1.Amount ELSE 0 END) AS BillingAsSupplier,
				SUM(CASE WHEN tb1.[Type] != @BillingAmountType THEN tb1.Amount ELSE 0 END) AS PaymentAsSupplier,
				tb1.CustomerCreditLimit AS CustomerCreditLimit,
				tb1.SupplierCreditLimit AS SupplierCreditLimit
			FROM TB1 tb1
			GROUP BY	tb1.SupplierProfileID,
					tb1.CurrencyID,
					tb1.CustomerCreditLimit,
					tb1.SupplierCreditLimit
				)
				
			INSERT INTO @Amounts	
			SELECT NULL AS CarrierID,
			   SupplierProfileID AS ProfileID,
			   (CASE WHEN Balance IS NOT NULL THEN Balance ELSE 0 END) AS Balance,
			   0.0 AS BillingAsCustomer,
			   (CASE WHEN BillingAsSupplier IS NOT NULL THEN BillingAsSupplier ELSE 0 END) AS BillingAsSupplier,
			   tb2.Currency,
			   tb2.CustomerCreditLimit,
			   tb2.SupplierCreditLimit,
			   0 AS Tolerance,
			   0.0 AS PaymentAsCustomer,
			   (CASE WHEN PaymentAsSupplier IS NOT NULL THEN PaymentAsSupplier ELSE 0 END)  AS PaymentAsSupplier
			FROM TB2
			
			SELECT 
				am.CarrierID,
				am.ProfileID,
				Sum(am.Balance) AS Balance,
				CONVERT(DECIMAL(10,2),SUM(am.BillingAsCustomer)) AS BillingAsCustomer,
				CONVERT(DECIMAL(10,2),SUM(am.BillingAsSupplier)) AS BillingAsSupplier,
				am.Currency,
				am.CustomerCreditLimit,
				am.SupplierCreditLimit,
				CASE WHEN Sum(am.Balance) > 0 THEN CONVERT(DECIMAL(10,2),( am.SupplierCreditLimit - SUM(am.Balance))) ELSE CONVERT(DECIMAL(10,2), (am.CustomerCreditLimit  + SUM(am.Balance))) END AS Tolerance,
				CONVERT(DECIMAL(10,2),SUM(am.PaymentAsCustomer)) AS PaymentAsCustomer,
				CONVERT(DECIMAL(10,2),SUM(am.PaymentAsSupplier)) AS PaymentAsSupplier
			FROM @Amounts am
			GROUP BY CarrierID, ProfileID, Currency, CustomerCreditLimit, SupplierCreditLimit
			ORDER BY Tolerance 			
		END
END

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON