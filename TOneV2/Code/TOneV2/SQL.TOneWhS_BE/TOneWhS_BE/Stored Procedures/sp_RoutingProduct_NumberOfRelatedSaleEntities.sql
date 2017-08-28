-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_RoutingProduct_NumberOfRelatedSaleEntities]
	@routingProductId int,
	@numberOfRelatedSaleEntities BigInt =0 out
AS
BEGIN
	BEGIN
		Select @numberOfRelatedSaleEntities=COUNT([ID]) FROM [TOneWhS_BE].[SaleEntityRoutingProduct] WHERE [RoutingProductID]=@routingProductId
	END
END