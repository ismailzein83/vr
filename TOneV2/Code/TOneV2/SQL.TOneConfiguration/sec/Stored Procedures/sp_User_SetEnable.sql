CREATE PROCEDURE [sec].[sp_User_SetEnable] 
	@ID int,
	@LastModifiedBy int
AS
BEGIN
	begin
		UPDATE sec.[User]
		SET EnabledTill = NULL, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
		WHERE ID = @ID
	end
END