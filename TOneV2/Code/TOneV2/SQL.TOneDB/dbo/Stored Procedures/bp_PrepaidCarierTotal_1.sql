-- ======================================================================
-- Author: Mazen Ghool		
-- Description: 
-- ======================================================================
CREATE PROCEDURE [dbo].[bp_PrepaidCarierTotal]
    @ShowCustomerTotal char(1) = 'Y',
	@ShowSupplierTotal char(1) = 'Y'
AS
BEGIN
	SET NOCOUNT ON;
	IF @ShowCustomerTotal = 'Y'
		SELECT     PA.CustomerID,
				   SUM(PA.Amount) AS Balance,
				   PA.CurrencyID, 
				   CA.CustomerCreditLimit,
				   SUM(PA.Amount) - CA.CustomerCreditLimit AS Tolerance
		FROM       CarrierAccount AS CA 
					INNER JOIN CarrierProfile AS CP ON CA.ProfileID = CP.ProfileID 
					INNER JOIN PrepaidAmount AS PA ON CA.CarrierAccountID = PA.CustomerID
		WHERE      (PA.CustomerID IS NOT NULL) 
		GROUP BY   PA.CustomerID,
				   PA.CurrencyID, 
				   CP.Name, 
				   CA.CustomerCreditLimit
		ORDER BY CP.Name
	IF @ShowSupplierTotal = 'Y'
		SELECT     PA.SupplierID,
				   SUM(PA.Amount) AS Balance,
				   PA.CurrencyID, 
				   CA.SupplierCreditLimit,
				   SUM(PA.Amount) - CA.SupplierCreditLimit AS Tolerance
		FROM       CarrierAccount AS CA 
					INNER JOIN CarrierProfile AS CP ON CA.ProfileID = CP.ProfileID 
					INNER JOIN PrepaidAmount AS PA ON CA.CarrierAccountID = PA.SupplierID
		WHERE      (PA.SupplierID IS NOT NULL) 
		GROUP BY   PA.SupplierID,
				   PA.CurrencyID, 
				   CP.Name, 
				   CA.SupplierCreditLimit
		ORDER BY CP.Name
END