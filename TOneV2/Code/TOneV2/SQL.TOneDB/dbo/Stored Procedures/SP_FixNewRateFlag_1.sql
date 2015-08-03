CREATE PROCEDURE [dbo].[SP_FixNewRateFlag]
AS
BEGIN
UPDATE Rate
SET	Change = 2
FROM Rate 
JOIN PriceList pl ON pl.PriceListID = Rate.PriceListID
WHERE 
NOT EXISTS 
(
	SELECT * FROM Rate pr JOIN PriceList prl ON prl.PriceListID = pr.PriceListID
		WHERE pr.ZoneID = Rate.ZoneID
			AND prl.SupplierID = pl.SupplierID
			AND prl.CustomerID = pl.CustomerID
			AND pr.BeginEffectiveDate < Rate.BeginEffectiveDate
			AND pr.EndEffectiveDate = Rate.BeginEffectiveDate
)
END