Create PROCEDURE [TOneWhS_BE].[sp_SellingNumberPlan_Update]
	@ID int,
	@Name nvarchar(255)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM TOneWhS_BE.[SellingNumberPlan] WHERE ID != @Id AND Name = @Name)
	BEGIN
		Update TOneWhS_BE.SellingNumberPlan
	Set Name = @Name
	Where ID = @ID
	END



	
END