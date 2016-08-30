CREATE PROCEDURE [sec].[sp_User_GetById] 
	@ID int
AS
BEGIN
	Select	[ID], [Name], [Email], [Status], LastLogin, [Description] 
	FROM	[sec].[User] WITH(NOLOCK) 
	WHERE	ID = @ID
END