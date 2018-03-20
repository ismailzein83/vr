CREATE PROCEDURE [sec].[sp_User_UpdateTempPasswordByEmail] 
    @email nvarchar(255),
	@TempPassword nvarchar(255),
	@ValidTill datetime,
	@LastModifiedBy int
AS
BEGIN

	UPDATE sec.[User]
	SET TempPassword = @TempPassword,  TempPasswordValidTill = @ValidTill, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
	WHERE Email   = @email

END