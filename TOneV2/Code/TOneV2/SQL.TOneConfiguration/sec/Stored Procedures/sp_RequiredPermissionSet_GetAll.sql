CREATE PROCEDURE [sec].[sp_RequiredPermissionSet_GetAll]
AS
BEGIN
	SELECT ID, Module, RequiredPermissionString FROM [sec].[RequiredPermissionSet] WITH (NOLOCK)
END