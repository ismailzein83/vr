﻿

CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetBetweenPeriod]
	@From DateTime,
	@Till DateTime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT   rate.[ID]
			, rate.[PriceListID]
			, rate.[ZoneID]
			, rate.[NormalRate]
			, rate.[OtherRates]
			, rate.[BED]
			, rate.[EED]
			,rate.CurrencyID
			,rate.change
	  FROM [TOneWhS_BE].SupplierRate rate 
	  Where (rate.EED is null and rate.BED<@Till) or(rate.EED>@From and rate.EED<@Till)
END