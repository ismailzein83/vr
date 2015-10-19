-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_RoutingProduct_Update]
	@ID int,
	@Name nvarchar(255),
	@SaleZonePackageID int,
	@Settings nvarchar(MAX)
AS
BEGIN

	Update TOneWhS_BE.RoutingProduct
	Set Name = @Name,
		SaleZonePackageID = @SaleZonePackageID,
		Settings = @Settings
	Where ID = @ID
END