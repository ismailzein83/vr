-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].[sp_SellingProduct_Insert]
	@Name nvarchar(255),
	@DefaultRoutingProductId INT,
	@SellingNumberPlanID int,
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN

	Insert into TOneWhS_BE.SellingProduct([Name],[DefaultRoutingProductID], SellingNumberPlanID, [Settings])
	Values(@Name,@DefaultRoutingProductId, @SellingNumberPlanID, @Settings)
	
	Set @Id = @@IDENTITY
END