-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SellingProduct_Insert]
	@Name nvarchar(255),
	@DefaultRoutingProductId INT,
	@SellingNumberPlanID int,
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN
IF NOT EXISTS(select 1 from TOneWhS_BE.SellingProduct where Name = @Name)
	BEGIN
	Insert into TOneWhS_BE.SellingProduct([Name],[DefaultRoutingProductID], SellingNumberPlanID, [Settings])
	Values(@Name,@DefaultRoutingProductId, @SellingNumberPlanID, @Settings)
	
	Set @Id = @@IDENTITY
	END
END