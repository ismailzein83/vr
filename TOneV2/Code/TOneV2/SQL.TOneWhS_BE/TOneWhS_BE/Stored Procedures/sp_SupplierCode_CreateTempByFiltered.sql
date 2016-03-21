CREATE PROCEDURE  [TOneWhS_BE].[sp_SupplierCode_CreateTempByFiltered]
(
	@TempTableName varchar(200),	
	@Code varchar(20),
	@SupplierID INT,
	@ZonesIDs varchar(max),
	@EffectiveOn DateTime
)
AS
BEGIN
	SET NOCOUNT ON
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
    BEGIN
		DECLARE @ZonesIDsTable TABLE (ZoneID INT)
		INSERT INTO @ZonesIDsTable (ZoneID)
		SELECT CONVERT(INT, ParsedString) FROM [TOneWhS_BE].[ParseStringList](@ZonesIDs)

		SELECT code.[ID],
			code.[Code],
			code.[ZoneID],
			code.[CodeGroupID],
			code.[BED],
			code.[EED]
		
		INTO #Result
		
		FROM [TOneWhS_BE].[SupplierCode] code INNER JOIN TOneWhS_BE.SupplierZone zone ON code.ZoneID = zone.ID

		WHERE zone.SupplierID = @SupplierID
		AND (@Code IS NULL OR code.Code LIKE '%' + @Code + '%')
		AND (@ZonesIDs IS NULL OR code.ZoneID in (SELECT ZoneID FROM @ZonesIDsTable))
		AND (@EffectiveOn IS NULL OR (code.BED < = @EffectiveOn AND (code.EED IS NULL OR code.EED > @EffectiveOn)));
			
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
		EXEC(@sql)
	END
	SET NOCOUNT OFF
END