

CREATE PROCEDURE [LCR].[sp_Code_GetCodeMatchesByDistinctCodes]
	@DistinctCodesWithPossibleMatches LCR.DistinctCodeWithPossibleMatchType READONLY,
	@ActiveSuppliersCodeInfo LCR.SuppliersCodeInfoType READONLY,
	@EffectiveOn datetime
AS
BEGIN
	SET NOCOUNT ON;
	WITH AllCodeMatches AS
	(
		SELECT  distinctCodes.DistinctCode AS Code, 
				Z.SupplierID, 
				c.Code AS SupplierCode, 
				c.ID AS SupplierCodeId, 
				C.ZoneID AS SupplierZoneId
		, ROW_NUMBER() OVER (PARTITION BY Z.SupplierID, distinctCodes.DistinctCode ORDER BY distinctCodes.PossibleMatch DESC) RowNumber
		FROM @DistinctCodesWithPossibleMatches distinctCodes 
		JOIN Code c WITH (NOLOCK) on distinctCodes.PossibleMatch = c.Code
		JOIN Zone z WITH (NOLOCK) on c.ZoneID = z.ZoneID
		JOIN @ActiveSuppliersCodeInfo sup on z.SupplierID = sup.SupplierID
		WHERE c.BeginEffectiveDate <= @EffectiveOn
			AND (c.EndEffectiveDate IS NULL OR c.EndEffectiveDate > @EffectiveOn)
	)
	SELECT * FROM AllCodeMatches WHERE RowNumber = 1
END