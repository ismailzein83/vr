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
		  ,sr.Rate
		  ,sr.PriceListID
		  ,sr.ZoneID
		  ,sr.BED
		  ,sr.EED
	  FROM [TOneWhS_BE].SupplierRate sr LEFT JOIN [TOneWhS_BE].SupplierZone sz ON sr.ZoneID=sz.ID 
	  Where  (sr.EED is null or sr.EED > @when)
		and sz.SupplierID=@SupplierId
END