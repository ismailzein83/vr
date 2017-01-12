-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetByDate]
	-- Add the parameters for the stored procedure here
	@SupplierId INT,
	@When DateTime
AS
BEGIN
	Declare @SupplierId_local int = @SupplierId
	Declare @When_local DateTime = @When

	SELECT  sr.[ID],sr.Rate,sr.RateTypeID,sr.PriceListID,sr.CurrencyId,sr.ZoneID,sr.BED,sr.EED,sr.Change
	FROM	[TOneWhS_BE].SupplierRate sr WITH(NOLOCK) 
			LEFT JOIN [TOneWhS_BE].SupplierPriceList pl WITH(NOLOCK) ON sr.PriceListID = pl.ID 
	Where  (sr.EED is null or sr.EED > @When_local)
			and pl.SupplierID = @SupplierId_local
END