CREATE PROCEDURE [dbo].[bp_ReportInvalidEntities]
AS
BEGIN

-- Invalid Rates (rates for the same zone and effective at overlapping dates)
SELECT * FROM Rate R1, Pricelist p1, Rate R2, PriceList p2 WHERE 
	R1.ZoneID = R2.ZoneID
	AND R1.PriceListID = P1.PriceListID
	AND R2.PriceListID = P2.PriceListID
	AND P1.CustomerID = P2.CustomerID
	AND P1.SupplierID = P2.SupplierID
	AND R1.RateID <> R2.RateID
	AND R1.BeginEffectiveDate BETWEEN R2.BeginEffectiveDate AND ISNULL(R2.EndEffectiveDate, getdate())

-- Invalid Codes (Code defined for the same zone and effective at overlapping dates)
SELECT * FROM Code C1, Code C2 WHERE 
	C1.ZoneID = C2.ZoneID
	AND C1.Code = C2.Code
	AND C1.ID <> C2.ID
	AND C1.BeginEffectiveDate BETWEEN C2.BeginEffectiveDate AND ISNULL(C2.EndEffectiveDate, getdate())

RETURN 0
END