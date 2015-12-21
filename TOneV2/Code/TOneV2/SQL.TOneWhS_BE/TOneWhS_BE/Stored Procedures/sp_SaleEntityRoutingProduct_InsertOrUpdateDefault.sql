-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityRoutingProduct_InsertOrUpdateDefault]
	--@NewSaleEntityRoutingProductsTable [TOneWhS_BE].[NewSaleEntityRoutingProduct] READONLY
	@OwnerType INT,
	@OwnerID INT,
	@RoutingProductID INT,
	@BED DATETIME,
	@EED DATETIME = NULL
AS
BEGIN
	UPDATE TOneWhS_BE.SaleEntityRoutingProduct
	SET RoutingProductID = @RoutingProductID, BED = @BED, EED = @EED
	WHERE OwnerType = @OwnerType AND OwnerID = @OwnerID AND ZoneID IS NULL
	
	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO TOneWhS_BE.SaleEntityRoutingProduct (OwnerType, OwnerID, RoutingProductID, BED, EED)
		VALUES (@OwnerType, @OwnerID, @RoutingProductID, @BED, @EED)
	END
END