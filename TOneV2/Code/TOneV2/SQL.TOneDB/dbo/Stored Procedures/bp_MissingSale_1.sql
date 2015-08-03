CREATE PROCEDURE [dbo].[bp_MissingSale]
	@Top int = 50,
	@sys_CDR_Pricing_CDRID bigint,
	@FromDate datetime,
	@TillDate DATETIME,
	@CustomerID VARCHAR(10) = NULL,
	@SupplierID VARCHAR(10) = NULL
WITH RECOMPILE

AS
BEGIN
	
	--SET ROWCOUNT @Top;

WITH Ids AS (
				SELECT id
				FROM   Billing_CDR_Sale bcs WITH(nolock,INDEX(IX_Billing_CDR_Sale_Attempt))
				WHERE  			bcs.Attempt >= @FromDate
							And bcs.Attempt < @TillDate
)

,Main AS(SELECT ID,convert(varchar(10),bcm.Attempt,120) Attempt,bcm.CustomerID,bcm.SupplierID,bcm.OurZoneID,bcm.SupplierZoneID,bcm.DurationInSeconds DurationInSeconds
FROM   Billing_CDR_Main bcm WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt))
JOIN CarrierAccount cac ON cac.CarrierAccountID = bcm.CustomerID 
WHERE bcm.Attempt >= @FromDate
		AND  bcm.Attempt < @TillDate
		AND bcm.ID <= @sys_CDR_Pricing_CDRID
		AND cac.RepresentsASwitch <> 'Y'
		AND cac.IsPassThroughCustomer <> 'Y'
		AND cac.IsPassThroughSupplier <> 'Y'
)
SELECT bcm.Attempt Attempt,bcm.CustomerID,bcm.SupplierID,bcm.OurZoneID,bcm.SupplierZoneID,Sum(bcm.DurationInSeconds) DurationInSeconds
FROM   Main bcm
WHERE NOT EXISTS (
		SELECT id
				FROM Ids bcs Where bcs.ID = bcm.ID
					   )
	Group By bcm.Attempt,bcm.CustomerID,bcm.SupplierID,bcm.OurZoneID,bcm.SupplierZoneID
	Order by bcm.CustomerID



--SELECT convert(varchar(10),bcm.Attempt,120) Attempt,bcm.CustomerID,bcm.SupplierID,bcm.OurZoneID,bcm.SupplierZoneID,Sum(bcm.DurationInSeconds) DurationInSeconds
--FROM   Billing_CDR_Main bcm WITH(NOLOCK,index(IX_Billing_CDR_Main_Attempt))
--JOIN CarrierAccount cac ON cac.CarrierAccountID = bcm.CustomerID 
--WHERE bcm.Attempt >= @FromDate
--		AND  bcm.Attempt < @TillDate
--		--AND (@CustomerID IS NULL OR bcm.CustomerID = @CustomerID)
--		--AND (@SupplierID IS NULL OR bcm.SupplierID = @SupplierID)
--		AND  NOT EXISTS (
--				SELECT id
--				FROM   Billing_CDR_Sale bcs WITH(nolock)
--				WHERE  			bcs.Attempt >= @FromDate
--							And bcs.Attempt < @TillDate
--							And bcs.ID = bcm.ID
--					   )
--		AND bcm.ID <= @sys_CDR_Pricing_CDRID
--		AND cac.RepresentsASwitch <> 'Y'
--		AND cac.IsPassThroughCustomer <> 'Y'
--		AND cac.IsPassThroughSupplier <> 'Y'
--	Group By convert(varchar(10),bcm.Attempt,120),bcm.CustomerID,bcm.SupplierID,bcm.OurZoneID,bcm.SupplierZoneID
--	Order by bcm.CustomerID
	
END