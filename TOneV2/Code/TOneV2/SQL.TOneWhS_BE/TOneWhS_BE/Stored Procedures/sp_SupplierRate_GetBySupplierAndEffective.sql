﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetBySupplierAndEffective]
	-- Add the parameters for the stored procedure here
	@SupplierId INT,
	@Effective DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT  sr.[ID]
		  ,sr.NormalRate
		  ,sr.OtherRates
		  ,sr.PriceListID
		  ,pl.CurrencyId
		  ,sr.ZoneID
		  ,sr.BED
		  ,sr.EED
		  ,sr.Change
	  FROM [TOneWhS_BE].SupplierRate sr 
	  LEFT JOIN [TOneWhS_BE].SupplierPriceList pl ON sr.PriceListID = pl.ID 
	  Where  (sr.BED<=@Effective and (sr.EED is null or sr.EED > @Effective))
			and pl.SupplierID = @SupplierId
END