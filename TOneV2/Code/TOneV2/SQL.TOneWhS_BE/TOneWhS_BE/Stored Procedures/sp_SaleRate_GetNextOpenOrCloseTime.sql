CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetNextOpenOrCloseTime]
	@effectiveDate dateTime
AS
BEGIN
SELECT min(NextOpenOrCloseTime) as NextOpenOrCloseTime FROM(
	SELECT min(BED) as NextOpenOrCloseTime
	FROM [TOneWhS_BE].[SaleRate] with(nolock)
	WHERE BED > @effectiveDate and BED <> EED

	UNION ALL
	
	SELECT min(EED) as NextOpenOrCloseTime
	FROM [TOneWhS_BE].[SaleRate] with(nolock)
	WHERE EED > @effectiveDate and BED <> EED)aux

END