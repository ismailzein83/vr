CREATE PROCEDURE [TOneWhS_BE].[sp_RateType_Update]
	@ID int,
	@Name nvarchar(255)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM TOneWhS_BE.[RateType] WHERE ID != @Id AND Name = @Name)
	BEGIN
		Update TOneWhS_BE.RateType
	Set Name = @Name
	Where ID = @ID
	END



	
END