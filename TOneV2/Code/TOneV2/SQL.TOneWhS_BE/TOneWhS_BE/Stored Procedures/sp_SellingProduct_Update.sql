-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].[sp_SellingProduct_Update]
	@ID int,
	@Name nvarchar(255),
	@DefaultRoutingProductId INT,
	@SellingNumberPlanId int,
	@Settings nvarchar(MAX)
AS
BEGIN

	Update TOneWhS_BE.SellingProduct
	Set Name = @Name,
		DefaultRoutingProductID=@DefaultRoutingProductId,
		SellingNumberPlanID = @SellingNumberPlanId,
		Settings = @Settings
	Where ID = @ID
END