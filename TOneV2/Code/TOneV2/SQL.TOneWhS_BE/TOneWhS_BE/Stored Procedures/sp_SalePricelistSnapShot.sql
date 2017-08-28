
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePricelistSnapShot]
@PriceListID as int
AS
BEGIN
SELECT  sc.[PriceListID],sc.[SnapShotDetail]
FROM	[TOneWhS_BE].[SalePriceListSnapShot] sc with(nolock)
WHERE	sc.PricelistId = @PriceListID
	
END