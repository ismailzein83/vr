CREATE PROCEDURE [TOneWhS_Sales].[sp_PricingTemplate_Insert]
	@Name nvarchar(255),
	@SellingNumberPlanId int,
	@Settings nvarchar(max),
	@id INT OUT
AS
BEGIN
SET @id = 0;
IF NOT EXISTS(SELECT 1 FROM [TOneWhS_Sales].[PricingTemplate] WHERE Name = @Name)
	BEGIN
		INSERT INTO [TOneWhS_Sales].[PricingTemplate] (Name, SellingNumberPlanId, Settings)
		VALUES (@Name, @SellingNumberPlanId, @Settings)

		SET @id = SCOPE_IDENTITY()
	END
END