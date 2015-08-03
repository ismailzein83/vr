CREATE PROCEDURE [dbo].[bp_PrepaidCarrierTotal_EA]
    @ShowCustomerTotal char(1) = 'Y',
    @ShowSupplierTotal char(1) = 'Y'
AS
BEGIN
    SET NOCOUNT ON;
    IF @ShowCustomerTotal = 'Y'
        SELECT     
            PA.CustomerID AS CarrierID,
            PA.CustomerProfileID AS ProfileID,
            SUM(PA.Amount) AS Balance,
            PA.CurrencyID AS Currency,
            --(case WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END) AS CreditLimit,
            --ABS(ISNULL((CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END),0)) - ABS(SUM(PA.Amount)) AS Tolerance Old Fadi
            SUM(PA.Amount) AS Tolerance,
            SUM(CASE WHEN PA.[Type] = 1 THEN PA.Amount ELSE 0 END) AS Billing,
            SUM(CASE WHEN PA.[Type] != 1 THEN PA.Amount ELSE 0 END) AS Payment
        FROM
            PrepaidAmount PA
                LEFT JOIN CarrierAccount CA ON CA.CarrierAccountID = PA.CustomerID
                LEFT JOIN CarrierProfile CP ON CP.ProfileID = PA.CustomerProfileID  
        WHERE
            (PA.CustomerID IS NOT NULL OR PA.CustomerProfileID IS NOT NULL)
            AND PA.Date >= (CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerActivateDate ELSE CP.CustomerActivateDate END)
            AND ((CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerDeactivateDate ELSE CP.CustomerDeactivateDate END) IS NULL
                OR (CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerDeactivateDate ELSE CP.CustomerDeactivateDate END) > PA.Date)
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
             --(CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierCreditLimit ELSE CP.SupplierCreditLimit END) AS CreditLimit,
             --ABS(ISNULL((CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierCreditLimit ELSE CP.SupplierCreditLimit END),0)) - ABS(SUM(PA.Amount)) AS Tolerance Old Fadi
             -SUM(PA.Amount) AS Tolerance,
             SUM(CASE WHEN PA.[Type] = 1 THEN PA.Amount ELSE 0 END) AS Billing,
             SUM(CASE WHEN PA.[Type] != 1 THEN PA.Amount ELSE 0 END) AS Payment
        FROM
            PrepaidAmount PA
            LEFT JOIN CarrierAccount CA ON CA.CarrierAccountID = PA.SupplierID
            LEFT JOIN CarrierProfile CP ON CP.ProfileID = PA.SupplierProfileID  
        WHERE
            (PA.SupplierID IS NOT NULL OR PA.SupplierProfileID IS NOT NULL)
            AND PA.Date >= (CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierActivateDate ELSE CP.SupplierActivateDate END)
            AND ((CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierDeactivateDate ELSE CP.SupplierDeactivateDate END) IS NULL
                OR (CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierDeactivateDate ELSE CP.SupplierDeactivateDate END) > PA.Date)
        GROUP BY   
            PA.SupplierProfileID,
            PA.SupplierID,
            PA.CurrencyID,
            (CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierCreditLimit ELSE CP.SupplierCreditLimit END)        
        ORDER BY Tolerance ASC
END