CREATE PROCEDURE [dbo].[bp_ErroneousEffectiveRates]
AS
	SELECT ZoneID, SupplierID, CustomerID, Min(R.BeginEffectiveDate) FirstRate, Max(R.BeginEffectiveDate) LastRate, Count(*) FROM Rate R, PriceList P 
		WHERE R.IsEffective = 'y'
			AND R.PriceListID = P.PriceListID
	GROUP BY ZoneID, SupplierID, CustomerID
	HAVING Count(*) > 1
	ORDER BY Count(*)