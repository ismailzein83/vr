CREATE PROCEDURE [sec].[sp_User_SetEnable] 
	@ID int
AS
BEGIN
	begin
		UPDATE sec.[User]
		SET EnabledTill = NULL
		WHERE ID = @ID
	end
END