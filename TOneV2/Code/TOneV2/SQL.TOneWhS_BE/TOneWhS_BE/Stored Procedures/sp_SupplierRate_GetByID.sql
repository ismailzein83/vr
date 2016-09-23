

CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetByID]
	@ID INT
AS
BEGIN

	SET NOCOUNT ON;

	SELECT  sr.[ID],sr.NormalRate,sr.OtherRates,sr.RateTypeID,sr.PriceListID,sr.CurrencyId,sr.ZoneID,sr.BED,sr.EED,sr.Change
	FROM	[TOneWhS_BE].SupplierRate sr WITH(NOLOCK) 
	Where  sr.ID = @ID
END