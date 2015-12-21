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
	  FROM [TOneWhS_BE].SupplierRate sr LEFT JOIN [TOneWhS_BE].SupplierPriceList pl ON sr.PriceListID = pl.ID 
	  Where  (sr.EED is null or sr.EED > @when)
		and pl.SupplierID=@SupplierId
END