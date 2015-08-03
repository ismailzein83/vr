CREATE PROCEDURE [dbo].[bp_MissingCost]
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
                  FROM   Billing_CDR_Cost bcc WITH(nolock,INDEX(IX_Billing_CDR_Cost_Attempt))
                  WHERE  			bcc.Attempt >= @FromDate
									And bcc.Attempt < @TillDate
)
,main As(SELECT ID,convert(varchar(10),bcm.Attempt,120) Attempt,bcm.CustomerID,bcm.SupplierID,bcm.OurZoneID,bcm.SupplierZoneID,bcm.DurationInSeconds DurationInSeconds
	FROM   Billing_CDR_Main bcm WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt))
    JOIN CarrierAccount cac ON cac.CarrierAccountID = bcm.customerID
    WHERE bcm.Attempt >= @FromDate
			AND  bcm.Attempt < @TillDate
			AND bcm.ID <= @sys_CDR_Pricing_CDRID
			AND cac.RepresentsASwitch <> 'Y' 
			AND cac.IsPassThroughCustomer <> 'Y'
			AND cac.IsPassThroughSupplier <> 'Y'
)
SELECT Attempt,bcm.CustomerID,bcm.SupplierID,bcm.OurZoneID,bcm.SupplierZoneID,Sum(bcm.DurationInSeconds) DurationInSeconds
	FROM   Main bcm
    WHERE NOT EXISTS (
                  SELECT id
                  FROM   Ids bcc
                  WHERE  bcc.ID = bcm.ID
					   )
			Group By bcm.Attempt,bcm.CustomerID,bcm.SupplierID,bcm.OurZoneID,bcm.SupplierZoneID
			order by bcm.SupplierID



--SELECT convert(varchar(10),bcm.Attempt,120) Attempt,bcm.CustomerID,bcm.SupplierID,bcm.OurZoneID,bcm.SupplierZoneID,Sum(bcm.DurationInSeconds) DurationInSeconds
--	FROM   Billing_CDR_Main bcm WITH(NOLOCK)
--    JOIN CarrierAccount cac ON cac.CarrierAccountID = bcm.customerID
--    WHERE bcm.Attempt >= @FromDate
--			AND  bcm.Attempt < @TillDate
--			--AND (@CustomerID IS NULL OR bcm.CustomerID = @CustomerID )
--			--AND (@SupplierID IS NULL OR bcm.SupplierID = @SupplierID)
--			AND  NOT EXISTS (
--                  SELECT id
--                  FROM   Billing_CDR_Cost bcc WITH(nolock)
--                  WHERE  			bcc.Attempt >= @FromDate
--									And bcc.Attempt < @TillDate
--									And bcc.ID = bcm.ID
--					   )
--			AND bcm.ID <= @sys_CDR_Pricing_CDRID
--			AND cac.RepresentsASwitch <> 'Y' 
--			AND cac.IsPassThroughCustomer <> 'Y'
--			AND cac.IsPassThroughSupplier <> 'Y'
--			Group By convert(varchar(10),bcm.Attempt,120),bcm.CustomerID,bcm.SupplierID,bcm.OurZoneID,bcm.SupplierZoneID
--			order by bcm.SupplierID


END