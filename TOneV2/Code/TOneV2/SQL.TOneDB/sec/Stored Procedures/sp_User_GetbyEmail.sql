CREATE PROCEDURE [sec].[sp_User_GetbyEmail] 
	@Email nvarchar(255)
AS
BEGIN
	Select [ID], [Name], [Email], [Password], [Status], LastLogin, [Description] FROM sec.[User]
	WHERE Email = @Email
END