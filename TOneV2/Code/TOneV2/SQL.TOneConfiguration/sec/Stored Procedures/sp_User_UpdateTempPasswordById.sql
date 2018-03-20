CREATE PROCEDURE [sec].[sp_User_UpdateTempPasswordById] 
    @userId int,
	@TempPassword nvarchar(255),
	@ValidTill datetime,
	@LastModifiedBy int
AS
BEGIN

	UPDATE sec.[User]
	SET TempPassword = @TempPassword,  TempPasswordValidTill = @ValidTill, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
	WHERE ID  = @userId

END