CREATE PROCEDURE [sec].[sp_User_GetbyId] 
	@ID int
AS
BEGIN
	Select [ID], [Name], [Email], [Password], [Status], LastLogin, [Description] FROM sec.[User]
	WHERE ID = @ID
END