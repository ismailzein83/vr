CREATE PROCEDURE [sec].[sp_User_GetByEmail] 
	@Email nvarchar(255)
AS
BEGIN
	Select	[ID], [Name], [Email], LastLogin, [Description], [TenantId] , [EnabledTill],ExtendedSettings, DisabledTill
	FROM	[sec].[User] WITH(NOLOCK) 
	WHERE	Email = @Email
END