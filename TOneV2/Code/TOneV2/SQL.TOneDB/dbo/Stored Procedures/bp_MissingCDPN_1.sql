CREATE PROCEDURE [dbo].[bp_MissingCDPN](@From datetime, @Till datetime, @Top int = 100, @MinDuration numeric(13,5) = 0)
WITH RECOMPILE
AS
BEGIN

	SET NOCOUNT ON
	
	--SET ROWCOUNT @Top 
	
    SELECT * 
    FROM Billing_CDR_Invalid bci  WITH(NOLOCK,INDEX(IX_Billing_CDR_Invalid_Attempt)) 
    JOIN CarrierAccount cac ON bci.CustomerID = cac.CarrierAccountID 
    WHERE 	bci.Attempt >=@From AND bci.Attempt <@Till
			AND bci.CustomerID IS NOT NULL 
			AND bci.SupplierID IS NOT NULL           
			AND bci.CDPN IS NULL
		    AND bci.DurationInSeconds > @MinDuration
			AND cac.RepresentsASwitch <> 'Y'
			AND cac.IsPassThroughCustomer <> 'Y'
			AND cac.IsPassThroughSupplier <> 'Y'
	RETURN 
END