CREATE PROCEDURE [sec].[sp_RequiredPermissionSet_InsertIfNotExists]
	@Module varchar(255),
	@RequiredPermissionString varchar(255)
AS
BEGIN
	INSERT INTO [sec].[RequiredPermissionSet]
	(Module, RequiredPermissionString)
	SELECT @Module, @RequiredPermissionString
	WHERE NOT EXISTS (SELECT NULL FROM [sec].[RequiredPermissionSet] WITH (NOLOCK) WHERE RequiredPermissionString = @RequiredPermissionString AND Module = @Module)

    SELECT ID FROM [sec].[RequiredPermissionSet] WITH (NOLOCK) WHERE RequiredPermissionString = @RequiredPermissionString AND Module = @Module
END