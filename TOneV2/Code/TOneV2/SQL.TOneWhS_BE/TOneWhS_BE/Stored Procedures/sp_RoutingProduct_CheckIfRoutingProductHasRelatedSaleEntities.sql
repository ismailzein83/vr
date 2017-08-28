-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_RoutingProduct_CheckIfRoutingProductHasRelatedSaleEntities]
	@routingProductId int
AS
BEGIN
		IF EXISTS(Select 1 FROM [TOneWhS_BE].[SaleEntityRoutingProduct] WHERE [RoutingProductID]=@routingProductId)
		SELECT 1
		ELSE
		SELECT 0;
END