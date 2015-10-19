-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].[sp_PricingProduct_Insert]
	@Name nvarchar(255),
	@DefaultRoutingProductId INT,
	@SaleZonePackageID int,
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN

	Insert into TOneWhS_BE.PricingProduct([Name],[DefaultRoutingProductID], [SaleZonePackageID], [Settings])
	Values(@Name,@DefaultRoutingProductId, @SaleZonePackageID, @Settings)
	
	Set @Id = @@IDENTITY
END