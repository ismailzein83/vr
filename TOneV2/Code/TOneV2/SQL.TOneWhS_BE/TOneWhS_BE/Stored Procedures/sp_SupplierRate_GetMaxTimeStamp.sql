CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetMaxTimeStamp]
AS
BEGIN
	SELECT max(timestamp) FROM [TOneWhS_BE].[SupplierRate] with(nolock)
END