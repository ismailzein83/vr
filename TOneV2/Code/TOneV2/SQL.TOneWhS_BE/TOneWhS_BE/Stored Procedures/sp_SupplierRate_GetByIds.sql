CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetByIds]
@SupplierRateIds VARCHAR(MAX)
AS
BEGIN
IF (@SupplierRateIds IS NOT NULL)
	BEGIN
		DECLARE @SupplierRateIdsTable AS TABLE (SupplierRateId BIGINT)
		INSERT INTO @SupplierRateIdsTable SELECT CONVERT(BIGINT, ParsedString) FROM [TOneWhS_BE].[ParseStringList](@SupplierRateIds)
	END;

	SELECT	[ID],[PriceListID],[ZoneID],[CurrencyID],[Rate],[RateTypeID],[Change],[BED],[EED]
	FROM	[TOneWhS_BE].SupplierRate WITH(NOLOCK) 
	Where ID in (SELECT SupplierRateId FROM @SupplierRateIdsTable)
END