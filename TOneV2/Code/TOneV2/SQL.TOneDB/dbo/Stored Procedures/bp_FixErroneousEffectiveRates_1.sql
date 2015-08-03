CREATE PROCEDURE [dbo].[bp_FixErroneousEffectiveRates]
AS
UPDATE Rate SET EndEffectiveDate = 
	(
		SELECT TOP 1 RN.BeginEffectiveDate FROM Rate RN, PriceList PN, PriceList PO 
			WHERE 
				PO.PricelistID = Rate.PriceListID
			AND PO.SupplierID = PN.SupplierID AND PO.CustomerID = PN.CustomerID 
			AND PN.PriceListID = RN.PriceListID
			AND RN.ZoneID = Rate.ZoneID 
			AND (
				(RN.BeginEffectiveDate > Rate.BeginEffectiveDate AND RN.RateID <> Rate.RateID) 
				OR 
				(RN.BeginEffectiveDate = Rate.BeginEffectiveDate AND RN.RateID > Rate.RateID)
				) 				
			AND RN.EndEffectiveDate IS NULL
			ORDER BY RN.BeginEffectiveDate ASC
	)
WHERE 
		Rate.EndEffectiveDate IS NULL
	AND EXISTS 
		(
			SELECT * FROM Rate RN, PriceList PN, PriceList PO 
				WHERE 
					PO.PricelistID = Rate.PriceListID
				AND PO.SupplierID = PN.SupplierID AND PO.CustomerID = PN.CustomerID 
				AND PN.PriceListID = RN.PriceListID
				AND RN.ZoneID = Rate.ZoneID 
				AND (
					(RN.BeginEffectiveDate > Rate.BeginEffectiveDate AND RN.RateID <> Rate.RateID) 
					OR 
					(RN.BeginEffectiveDate = Rate.BeginEffectiveDate AND RN.RateID > Rate.RateID)
					) 				
				AND RN.EndEffectiveDate IS NULL 
		)