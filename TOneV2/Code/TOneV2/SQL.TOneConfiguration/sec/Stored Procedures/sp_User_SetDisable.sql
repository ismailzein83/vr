CREATE PROCEDURE [sec].[sp_User_SetDisable] 
	@ID int
AS
BEGIN
	begin
		UPDATE sec.[User]
		SET EnabledTill = DATEADD(MINUTE,-5,GETDATE()) /* Add -5 minutes to avoid the DATETIME.NOW from server side */
		WHERE ID = @ID
	end
END