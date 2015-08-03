CREATE PROCEDURE [dbo].[bp_MissSupplierZone](
    @From         DATETIME,
    @Till         DATETIME,
    @Top          INT = 100,
    @MinDuration  NUMERIC(13, 5) = 0,
    @CustomerID VARCHAR(10) = NULL ,
    @SupplierID VARCHAR(10) = NULL 
)
WITH RECOMPILE
AS
BEGIN
    
	SET NOCOUNT ON
	
	--SET ROWCOUNT @Top 
	SELECT convert(varchar(10),bci.Attempt,120) Attempt,bci.CustomerID,bci.SupplierID,bci.OurZoneID,bci.SupplierZoneID,Sum(bci.DurationInSeconds) DurationInSeconds
	FROM Billing_CDR_Invalid bci WITH(NOLOCK,INDEX(IX_Billing_CDR_Invalid_Attempt))
		JOIN CarrierAccount cac ON bci.CustomerID = cac.CarrierAccountID
		JOIN CarrierAccount cas ON bci.SupplierID = cas.CarrierAccountID
	WHERE  bci.Attempt >=@From AND bci.Attempt <@Till
        AND (@CustomerID IS NULL OR bci.CustomerID = @CustomerID)
        AND (@SupplierID IS NULL OR bci.SupplierID = @SupplierID)
        AND (bci.DurationInSeconds > @MinDuration)
        AND bci.CustomerID IS NOT NULL
        AND bci.SupplierID IS NOT NULL
        AND bci.SupplierZoneID IS NULL 		
        AND cac.RepresentsASwitch <> 'Y'
        AND cas.RepresentsASwitch <> 'Y'
		AND cac.IsPassThroughCustomer <> 'Y' 
		AND cac.IsPassThroughSupplier <> 'Y'
	Group By convert(varchar(10),bci.Attempt,120),bci.CustomerID,bci.SupplierID,bci.OurZoneID,bci.SupplierZoneID
	order by bci.SupplierID
	
	RETURN
END