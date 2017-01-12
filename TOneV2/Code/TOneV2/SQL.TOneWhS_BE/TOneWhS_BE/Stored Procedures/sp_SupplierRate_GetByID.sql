

CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetByID]
	@ID INT
AS
BEGIN

	DECLARE @ID_local INT = @ID

	SELECT  sr.[ID],sr.Rate,sr.RateTypeID,sr.PriceListID,sr.CurrencyId,sr.ZoneID,sr.BED,sr.EED,sr.Change
	FROM	[TOneWhS_BE].SupplierRate sr WITH(NOLOCK) 
	Where  sr.ID = @ID_local
END