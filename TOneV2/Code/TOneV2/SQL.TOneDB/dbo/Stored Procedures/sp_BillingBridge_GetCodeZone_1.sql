-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_BillingBridge_GetCodeZone]
	@When DATETIME,
	@SupplierID VARCHAR(10) = NULL,
	@CustomerID VARCHAR(10) = NULL,
	@zoneID INT = NULL
AS
BEGIN
;with 
_PriceList AS
(
    SELECT P.PriceListID 
    FROM PriceList P WITH(NOLOCK)
    WHERE P.CustomerID = @CustomerID  and supplierid = @SupplierID
),
EffPriceList AS
(
    SELECT P.PriceListID 
    FROM PriceList P WITH(NOLOCK)
    WHERE P.CustomerID = @CustomerID AND P.IsEffective='Y' AND SupplierID= @SupplierID
),
Ratez AS
(
    SELECT r.ZoneID
    FROM Rate r 
    WHERE r.PriceListID IN (SELECT PriceListID FROM _PriceList )
    AND ( (@zoneID IS NULL AND R.ZoneID NOT IN ( SELECT ZoneID FROM ToDConsideration TOD WHERE customerid=@CustomerID and supplierid=@SupplierID and (tod.EndEffectiveDate >= @When ) ) ) OR R.ZoneID = @zoneID)
  --  AND ( r.EndEffectiveDate >= @When  )
)
,EffRatez AS
(
    SELECT r.ZoneID
    FROM Rate r 
    WHERE r.PriceListID IN (SELECT PriceListID FROM EffPriceList )
    AND ( (@zoneID IS NULL AND R.ZoneID NOT IN ( SELECT ZoneID FROM ToDConsideration TOD WHERE customerid=@CustomerID and supplierid=@SupplierID and tod.iseffective='y') ) OR R.ZoneID = @zoneID)
  --  AND R.IsEffective='Y'
)
,Codez AS
 (
    SELECT  C.ZoneID,Z.Name,Z.CodeGroup,C.Code,C.BeginEffectiveDate,C.EndEffectiveDate
    FROM Code C WITH(NOLOCK)
    JOIN zone z ON z.ZoneID = c.ZoneID
    WHERE (c.EndEffectiveDate is null or  C.EndEffectiveDate >= @When )
    AND c.ZoneID IN ( SELECT ZoneID FROM Ratez)
    AND C.ZoneID NOT IN (SELECT ZoneID FROM Tariff t WHERE ( t.EndEffectiveDate >= @When ) and t.customerid=@CustomerID and t.supplierid=@SupplierID )
 ) 
 ,EffCodez AS
 (
    SELECT  C.ZoneID,Z.Name,Z.CodeGroup,C.Code,C.BeginEffectiveDate,C.EndEffectiveDate
    FROM Code C WITH(NOLOCK)
    JOIN zone z ON z.ZoneID = c.ZoneID
    WHERE c.ZoneID IN ( SELECT ZoneID FROM EffRatez)
    AND C.IsEffective='Y'
    AND C.ZoneID NOT IN (SELECT ZoneID FROM Tariff t WHERE t.IsEffective='Y' and t.customerid=@CustomerID and t.supplierid=@SupplierID )
 ) 
,_Finalz
AS  
(
    SELECT * FROM Codez
    UNION SELECT * FROM EffCodez
)
SELECT distinct r.ZoneID,r.Name,r.CodeGroup,r.Code,r.BeginEffectiveDate,r.EndEffectiveDate FROM _Finalz r
where ( r.EndEffectiveDate IS null or r.EndEffectiveDate <> r.BeginEffectiveDate)
	
END