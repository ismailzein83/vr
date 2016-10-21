

CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetByID]
	@ID INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT  sr.[ID],sr.RateTypeID,sr.Rate,sr.PriceListID,sr.ZoneID,sr.BED,sr.EED,sr.Change, sr.CurrencyID
	FROM	[TOneWhS_BE].SaleRate sr WITH(NOLOCK) 
	Where  sr.ID = @ID
END