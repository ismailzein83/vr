CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetMaxTimeStamp]
AS
BEGIN
	SELECT max(timestamp) FROM [TOneWhS_BE].[SaleRate] with(nolock)
END