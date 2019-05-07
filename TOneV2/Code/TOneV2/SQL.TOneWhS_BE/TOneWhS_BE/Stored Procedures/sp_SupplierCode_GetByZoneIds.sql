
-- ===================================================
-- Description:	SP to get all SupplierCodes by ZoneIds
-- ===================================================

CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierCode_GetByZoneIds]
	@ZonesIDs varchar(max),
	@effectiveDate DateTime
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ZonesIDsTable TABLE (ZoneID INT)
	INSERT INTO @ZonesIDsTable (ZoneID)
	SELECT CONVERT(INT, ParsedString) FROM [TOneWhS_BE].[ParseStringList](@ZonesIDs)
		
	SELECT  [ID],[Code],[ZoneID],[BED],[EED],[CodeGroupID],[SourceID]
	FROM	[TOneWhS_BE].[SupplierCode] sc WITH(NOLOCK) 
	WHERE	[ZoneID] IN (SELECT ZoneID FROM @ZonesIDsTable)
			AND (sc.EED IS NULL OR sc.EED > @effectiveDate)
        
END