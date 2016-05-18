CREATE PROCEDURE [sec].[sp_User_UpdateTempPasswordById] 
    @userId int,
	@TempPassword nvarchar(255),
	@ValidTill datetime
AS
BEGIN

	UPDATE sec.[User]
	SET TempPassword = @TempPassword,  TempPasswordValidTill = @ValidTill
	WHERE ID  = @userId

END