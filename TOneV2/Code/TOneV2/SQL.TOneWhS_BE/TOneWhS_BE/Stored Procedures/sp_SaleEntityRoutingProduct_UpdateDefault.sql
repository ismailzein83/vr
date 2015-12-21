-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityRoutingProduct_UpdateDefault]
	@OwnerType INT,
	@OwnerID INT,
	@EED DATETIME = NULL
AS
BEGIN
	UPDATE TOneWhS_BE.SaleEntityRoutingProduct
	SET EED = @EED
	WHERE OwnerType = @OwnerType AND OwnerID = @OwnerID AND ZoneID IS NULL
END