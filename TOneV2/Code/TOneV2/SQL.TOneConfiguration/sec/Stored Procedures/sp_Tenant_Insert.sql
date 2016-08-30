CREATE PROCEDURE [sec].[sp_Tenant_Insert] 
	
	@Name Nvarchar(255),
	@Settings Nvarchar(max),
	@ParentTenantID int,
	@Id int out
AS
BEGIN
IF NOT EXISTS(select null from sec.[Tenant] where Name = @Name)
	BEGIN
		Insert into sec.[Tenant] ([Name],[Settings],[ParentTenantID])
		values(@Name, @Settings, @ParentTenantID)
		
		SET @Id = SCOPE_IDENTITY()
	END
END