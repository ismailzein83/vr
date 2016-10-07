CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierCode_GetFiltered]
(
	@Code varchar(20),
	@SupplierID INT,
	@ZonesIDs varchar(max),
	@EffectiveOn DateTime
)
AS
BEGIN
		DECLARE @ZonesIDsTable TABLE (ZoneID INT)
		INSERT INTO @ZonesIDsTable (ZoneID)
		SELECT CONVERT(INT, ParsedString) FROM [TOneWhS_BE].[ParseStringList](@ZonesIDs)

		SELECT code.[ID],code.[Code],code.[ZoneID],code.[CodeGroupID],code.[BED],code.[EED],code.CodeGroupID,code.SourceID		
		FROM	[TOneWhS_BE].[SupplierCode] code WITH(NOLOCK) 
				INNER JOIN TOneWhS_BE.SupplierZone zone WITH(NOLOCK) ON code.ZoneID = zone.ID
		WHERE	zone.SupplierID = @SupplierID
				AND (@Code IS NULL OR code.Code LIKE @Code + '%')
				AND (@ZonesIDs IS NULL OR code.ZoneID in (SELECT ZoneID FROM @ZonesIDsTable))
				AND (code.BED < = @EffectiveOn AND (code.EED IS NULL OR code.EED > @EffectiveOn));
			

END