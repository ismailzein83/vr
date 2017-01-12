-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetBySupplierAndEffective]
	@SupplierId INT,
	@Effective DateTime
AS
BEGIN
	DECLARE @SupplierId_local INT = @SupplierId
	DECLARE @Effective_local DateTime = @Effective

	SELECT  sr.[ID],sr.Rate,sr.PriceListID,sr.RateTypeID,sr.CurrencyId,sr.ZoneID,sr.BED,sr.EED,sr.Change
	FROM	[TOneWhS_BE].SupplierRate sr WITH(NOLOCK) 
			LEFT JOIN [TOneWhS_BE].SupplierPriceList pl WITH(NOLOCK) ON sr.PriceListID = pl.ID 
	Where	(sr.BED<=@Effective_local and (sr.EED is null or sr.EED > @Effective_local))
			and pl.SupplierID = @SupplierId_local
END