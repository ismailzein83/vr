-- =============================================
-- Author:		<Author,,Name>

-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_SPL].[sp_SupplierPriceListTemplate_Update]
	@ID INT,
	@SupplierId int,
	@ConfigDetails  nvarchar(MAX),
	@Draft  nvarchar(MAX)
AS
BEGIN

	UPDATE [TOneWhS_SPL].SupplierPriceListTemplate
	SET  ConfigDetails = @ConfigDetails , Draft = @Draft
	WHERE ID = @ID and SupplierID = @SupplierId

END