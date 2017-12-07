create PROCEDURE [sec].[sp_User_Unlock] 
	@ID int
AS
BEGIN
	begin
		UPDATE sec.[User]
		SET DisabledTill = NULL
		WHERE ID = @ID
	end
END