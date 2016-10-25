-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_RoutingProduct_Insert]
	@Name nvarchar(255),
	@SellingNumberPlanID int,
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM TOneWhS_BE.RoutingProduct WHERE [Name] = @Name)
	BEGIN
		Insert into TOneWhS_BE.RoutingProduct ([Name], [SellingNumberPlanID], [Settings])
		Values(@Name, @SellingNumberPlanID, @Settings)
	
		Set @Id = SCOPE_IDENTITY()
	END
END