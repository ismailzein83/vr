
-- ======================================================================
-- Author: Mohammad El-Shab
-- Description: 
-- ======================================================================
CREATE PROCEDURE [dbo].[EA_PrepaidCarrierTotal]
    @ShowCustomerTotal char(1) = 'Y',
	@ShowSupplierTotal char(1) = 'Y',
	@CarrierAccountID  varchar(5) = NULL,
	@CarrierProfileID  int 
AS
BEGIN
	SET NOCOUNT ON;
	IF @ShowCustomerTotal = 'Y'
		SELECT     
			PA.CustomerID AS CarrierID,
			PA.CustomerProfileID AS ProfileID,
			SUM(PA.Amount) AS Balance,
			PA.CurrencyID AS Currency,
			(case WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END) AS CreditLimit,
			SUM(PA.Amount) - ISNULL((CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END),0) AS Tolerance
		FROM
			PrepaidAmount PA 
				LEFT JOIN CarrierAccount CA ON CA.CarrierAccountID = PA.CustomerID
				LEFT JOIN CarrierProfile CP ON CP.ProfileID = PA.CustomerProfileID  
		WHERE
			PA.CustomerID = @CarrierAccountID OR PA.CustomerProfileID = @CarrierProfileID
		GROUP BY	
			PA.CustomerProfileID,
			PA.CustomerID,
			PA.CurrencyID, 
			(CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END)
		ORDER BY Tolerance ASC
	IF @ShowSupplierTotal = 'Y'
		SELECT
		     PA.SupplierID AS CarrierID,
			 PA.SupplierProfileID AS ProfileID,
			 SUM(PA.Amount) AS Balance,
			 PA.CurrencyID AS Currency,
			 (CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierCreditLimit ELSE CP.SupplierCreditLimit END) AS CreditLimit,
			 SUM(PA.Amount) - ISNULL((CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierCreditLimit ELSE CP.SupplierCreditLimit END),0) AS Tolerance
		FROM 
			PrepaidAmount PA 
		    LEFT JOIN CarrierAccount CA ON CA.CarrierAccountID = PA.SupplierID
			LEFT JOIN CarrierProfile CP ON CP.ProfileID = PA.CustomerProfileID  
		WHERE
		    PA.SupplierID = @CarrierAccountID OR PA.SupplierProfileID = @CarrierProfileID 
		GROUP BY   
			PA.SupplierProfileID,
			PA.SupplierID,
			PA.CurrencyID, 
			(CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierCreditLimit ELSE CP.SupplierCreditLimit END)		
		ORDER BY Tolerance ASC
END