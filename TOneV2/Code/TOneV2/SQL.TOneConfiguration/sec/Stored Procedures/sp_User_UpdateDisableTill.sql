CREATE PROCEDURE [sec].[sp_User_UpdateDisableTill] 
	@ID int,
	@DisableTill Nvarchar(255),
	@LastModifiedBy int
AS
BEGIN
	UPDATE sec.[User]
	SET  DisabledTill = @DisableTill, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
	WHERE ID = @ID
END