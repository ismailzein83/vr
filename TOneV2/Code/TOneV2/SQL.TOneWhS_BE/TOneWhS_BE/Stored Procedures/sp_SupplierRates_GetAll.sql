CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRates_GetAll]
	
AS
BEGIN

	SELECT	[ID]
		   ,[PriceListID]
		   ,[ZoneID]
		   ,[CurrencyID]
		   ,[Rate]
		   ,[RateTypeID]
		   ,[Change]
		   ,[BED]
		   ,[EED]
	FROM	[TOneWhS_BE].SupplierRate WITH(NOLOCK) 

END