-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_BillingBridge_GetZoneRate]
	@When DATETIME,
	@SupplierID VARCHAR(10) = NULL,
	@CustomerID VARCHAR(10) = NULL,
	@ZoneID INT = NULL
	
AS
BEGIN
;WITH 
EffPriceList AS
(
	SELECT pricelistid FROM PriceList pl WHERE pl.CustomerID = @CustomerID AND pl.IsEffective= 'Y' AND pl.SupplierID=@SupplierID
)
,_PriceList AS
(
	SELECT pricelistid FROM PriceList pl WHERE pl.CustomerID = @CustomerID AND pl.SupplierID=@SupplierID
)
,
EffectiveResult AS 
(
	SELECT R.ZoneID,R.BeginEffectiveDate,R.EndEffectiveDate,R.Rate,R.OffPeakRate,R.WeekendRate ,R.IsEffective
	FROM RATE R 
	JOIN EffPriceList P ON r.PriceListID = p.pricelistid
	AND ( (@zoneID IS NULL AND r.ZoneID NOT IN ( SELECT ZoneID FROM ToDConsideration TOD  where iseffective='y'  and customerid=@CustomerID and supplierid=@SupplierID) ) OR r.ZoneID = @zoneID)
	WHERE  r.IsEffective ='Y'
)
,FullResult AS 
(
SELECT * FROM effectiveresult 
UNION 
(
    SELECT R.ZoneID,R.BeginEffectiveDate,R.EndEffectiveDate,R.Rate,R.OffPeakRate,R.WeekendRate ,R.IsEffective
    FROM RATE R 
    JOIN _PriceList P ON r.PriceListID = p.pricelistid
    WHERE (r.endeffectivedate is null or  r.EndEffectiveDate > @When )
    AND ( (@zoneID IS NULL AND r.ZoneID NOT IN ( SELECT ZoneID FROM ToDConsideration TOD WHERE   endeffectivedate>@When and customerid=@CustomerID and supplierid=@SupplierID) ) OR r.ZoneID = @zoneID)
	)
)
SELECT  r.zoneid,z.name,R.BeginEffectiveDate,R.EndEffectiveDate,R.Rate,R.OffPeakRate,R.WeekendRate ,R.IsEffective 
FROM FullResult r 
join zone z on z.zoneid = r.zoneid
where ( R.EndEffectiveDate IS null or R.EndEffectiveDate <> R.BeginEffectiveDate)
and z.supplierid = @SupplierID
END