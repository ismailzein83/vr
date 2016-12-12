CREATE PROCEDURE [sec].[sp_User_GetByEmail] 
	@Email nvarchar(255)
AS
BEGIN
	Select	[ID], [Name], [Email], [Password], LastLogin, [Description] 
	FROM	[sec].[User] WITH(NOLOCK) 
	WHERE	Email = @Email
END