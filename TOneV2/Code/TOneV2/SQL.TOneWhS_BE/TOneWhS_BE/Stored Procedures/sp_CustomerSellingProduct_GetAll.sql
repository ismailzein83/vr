CREATE PROCEDURE [TOneWhS_BE].[sp_CustomerSellingProduct_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

			SELECT
				   cpp.[ID]
				  ,cpp.CustomerID
				  ,cpp.SellingProductID
				  ,cpp.AllDestinations
				  ,cpp.BED
				  ,cpp.EED
				  ,ca.Name as CustomerName
				  ,sp.Name as SellingProductName
			FROM TOneWhS_BE.CustomerSellingProduct cpp  
			Join TOneWhS_BE.CarrierAccount ca ON cpp.CustomerID=ca.ID 
			Join TOneWhS_BE.CarrierProfile cp ON ca.CarrierProfileID=cp.ID
			Join TOneWhS_BE.SellingProduct sp ON cpp.SellingProductID=sp.ID                              
           

	SET NOCOUNT OFF
END