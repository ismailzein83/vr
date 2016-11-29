CREATE PROCEDURE [VR_NumberingPlan].[sp_SellingNumberPlan_Update]
	@ID int,
	@Name nvarchar(255)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM VR_NumberingPlan.[SellingNumberPlan] WHERE ID != @Id AND Name = @Name)
	BEGIN
		Update VR_NumberingPlan.SellingNumberPlan
	Set Name = @Name
	Where ID = @ID
	END



	
END