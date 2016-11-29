CREATE PROCEDURE [VR_NumberingPlan].[sp_SellingNumberPlan_Insert]
	@Name nvarchar(255),
	@id INT OUT
AS

BEGIN
SET @id =0;
IF NOT EXISTS(SELECT 1 FROM VR_NumberingPlan.[SellingNumberPlan] WHERE Name = @Name)
	BEGIN
		INSERT INTO VR_NumberingPlan.SellingNumberPlan(Name)
		VALUES (@Name)

		SET @id = SCOPE_IDENTITY()
	END
END


/*sp_SellingNumberPlan_GetAll*/