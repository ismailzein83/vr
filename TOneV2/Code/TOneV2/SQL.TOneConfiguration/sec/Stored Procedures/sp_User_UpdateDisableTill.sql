create PROCEDURE [sec].[sp_User_UpdateDisableTill] 
	@ID int,
	@DisableTill Nvarchar(255)
AS
BEGIN
	UPDATE sec.[User]
	SET  DisabledTill = @DisableTill
	WHERE ID = @ID
END