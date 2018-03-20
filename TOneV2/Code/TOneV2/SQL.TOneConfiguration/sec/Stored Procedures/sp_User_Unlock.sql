CREATE PROCEDURE [sec].[sp_User_Unlock] 
	@ID int,
	@LastModifiedBy int
AS
BEGIN
	begin
		UPDATE sec.[User]
		SET DisabledTill = NULL, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
		WHERE ID = @ID
	end
END