CREATE PROCEDURE [sec].[sp_User_GetByEmail] 
	@Email nvarchar(255)
AS
BEGIN
	Select [ID], [Name], [Email], [Password], [Status], LastLogin, [Description] FROM sec.[User]
	WHERE Email = @Email
END