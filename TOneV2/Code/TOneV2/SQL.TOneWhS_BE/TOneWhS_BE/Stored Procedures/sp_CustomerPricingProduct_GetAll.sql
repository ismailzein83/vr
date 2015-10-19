CREATE PROCEDURE [TOneWhS_BE].[sp_CustomerPricingProduct_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

			SELECT
				   cpp.[ID]
				  ,cpp.CustomerID
				  ,cpp.PricingProductID
				  ,cpp.AllDestinations
				  ,cpp.BED
				  ,cpp.EED
				  ,ca.Name as CustomerName
				  ,pp.Name as PricingProductName
			FROM TOneWhS_BE.CustomerPricingProduct cpp  
			Join TOneWhS_BE.CarrierAccount ca ON cpp.CustomerID=ca.ID 
			Join TOneWhS_BE.CarrierProfile cp ON ca.CarrierProfileID=cp.ID
			Join TOneWhS_BE.PricingProduct pp ON cpp.PricingProductID=pp.ID                              
           

	SET NOCOUNT OFF
END