CREATE PROCEDURE [sec].[sp_User_GetTempPassword] 
                       
	@ID int
AS
BEGIN
	Select	[TempPassword] 
	FROM	[sec].[User] WITH(NOLOCK) 
	WHERE	ID = @ID
			and (TempPasswordValidTill is null or TempPasswordValidTill > GETDATE())
END