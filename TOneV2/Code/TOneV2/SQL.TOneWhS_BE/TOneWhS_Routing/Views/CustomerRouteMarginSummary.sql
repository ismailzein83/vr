CREATE VIEW TOneWhS_Routing.CustomerRouteMarginSummary AS
SELECT c.ID
	,c.CustomerID
	,c.SaleZoneID
	,c.SaleRate

	,c.MinSupplierRate			AS CurrentMinSupplierRate
	,c.MaxMargin				AS CurrentMaxMargin
	,c.MaxMarginCategoryID		AS CurrentMaxMarginCategoryID
	,f.MinSupplierRate			AS FutureMinSupplierRate
	,f.MaxMargin				AS FutureMaxMargin
	,f.MaxMarginCategoryID		AS FutureMaxMarginCategoryID
	,f.MaxMargin - c.MaxMargin	AS MaxMarginDiff
	,CASE
		WHEN f.MaxMargin - c.MaxMargin > 0 THEN CAST('780868AA-1328-494E-B774-C44C0136CD5A' AS UNIQUEIDENTIFIER)
		WHEN f.MaxMargin - c.MaxMargin < 0 THEN CAST('93EE1F4D-BE51-47FE-ABF1-B642CC13CB99' AS UNIQUEIDENTIFIER)
		ELSE									CAST('DC965B07-DB6E-4746-872E-89C138153994' AS UNIQUEIDENTIFIER)
	END AS MaxMarginComparison

	,c.MaxSupplierRate			AS CurrentMaxSupplierRate
	,c.MinMargin				AS CurrentMinMargin
	,c.MinMarginCategoryID		AS CurrentMinMarginCategoryID
	,f.MaxSupplierRate			AS FutureMaxSupplierRate
	,f.MinMargin				AS FutureMinMargin
	,f.MinMarginCategoryID		AS FutureMinMarginCategoryID
	,f.MinMargin - c.MinMargin	AS MinMarginDiff
	,CASE
		WHEN f.MinMargin - c.MinMargin > 0 THEN CAST('780868AA-1328-494E-B774-C44C0136CD5A' AS UNIQUEIDENTIFIER)
		WHEN f.MinMargin - c.MinMargin < 0 THEN CAST('93EE1F4D-BE51-47FE-ABF1-B642CC13CB99' AS UNIQUEIDENTIFIER)
		ELSE									CAST('DC965B07-DB6E-4746-872E-89C138153994' AS UNIQUEIDENTIFIER)
	END AS MinMarginComparison

	,c.AvgSupplierRate			AS CurrentAvgSupplierRate
	,c.AvgMargin				AS CurrentAvgMargin
	,c.AvgMarginCategoryID		AS CurrentAvgMarginCategoryID
	,f.AvgSupplierRate			AS FutureAvgSupplierRate
	,f.AvgMargin				AS FutureAvgMargin
	,f.AvgMarginCategoryID		AS FutureAvgMarginCategoryID
	,f.AvgMargin - c.AvgMargin	AS AvgMarginDiff
	,CASE
		WHEN f.AvgMargin - c.AvgMargin > 0 THEN CAST('780868AA-1328-494E-B774-C44C0136CD5A' AS UNIQUEIDENTIFIER)
		WHEN f.AvgMargin - c.AvgMargin < 0 THEN CAST('93EE1F4D-BE51-47FE-ABF1-B642CC13CB99' AS UNIQUEIDENTIFIER)
		ELSE									CAST('DC965B07-DB6E-4746-872E-89C138153994' AS UNIQUEIDENTIFIER)
	END AS AvgMarginComparison

	,f.CreatedTime				AS CreatedTime

From		[TOneWhS_Routing].[CustomerRouteMarginSummary_Future]	AS f
LEFT JOIN	[TOneWhS_Routing].[CustomerRouteMarginSummary_Current]	AS c
ON c.CustomerID = f.CustomerID AND c.SaleZoneID = f.SaleZoneID