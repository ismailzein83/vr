Create PROCEDURE [TOneWhS_Sales].[sp_PricingTemplate_Update]
	@ID int,
	@Name nvarchar(255), 
	@Settings nvarchar(max)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM [TOneWhS_Sales].[PricingTemplate] WHERE ID != @ID AND Name = @Name)
	BEGIN
		Update [TOneWhS_Sales].[PricingTemplate]
		Set Name = @Name, Settings = @Settings
		Where ID = @ID
	END
END