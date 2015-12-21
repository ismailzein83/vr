CREATE PROCEDURE [TOneWhS_BE].[sp_Switch_Delete]
	@ID int
AS
BEGIN

	Delete TOneWhS_BE.Switch
	Where ID = @ID
END