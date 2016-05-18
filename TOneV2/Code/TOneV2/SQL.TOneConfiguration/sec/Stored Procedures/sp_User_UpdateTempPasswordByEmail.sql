CREATE PROCEDURE [sec].[sp_User_UpdateTempPasswordByEmail] 
    @email nvarchar(255),
	@TempPassword nvarchar(255),
	@ValidTill datetime
AS
BEGIN

	UPDATE sec.[User]
	SET TempPassword = @TempPassword,  TempPasswordValidTill = @ValidTill
	WHERE Email   = @email

END