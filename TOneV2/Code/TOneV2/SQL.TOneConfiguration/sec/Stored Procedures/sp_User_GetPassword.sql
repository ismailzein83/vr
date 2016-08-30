CREATE PROCEDURE [sec].[sp_User_GetPassword] 
	@ID int
AS
BEGIN
	Select	[Password] 
	FROM	[sec].[User] WITH(NOLOCK) 
	WHERE	ID = @ID
END