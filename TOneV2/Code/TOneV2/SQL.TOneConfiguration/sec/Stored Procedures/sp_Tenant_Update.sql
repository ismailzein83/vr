CREATE PROCEDURE [sec].[sp_Tenant_Update] 
	@ID int,
	@Name Nvarchar(255),
	@Settings Nvarchar(max),
	@ParentTenantID int
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM sec.[Tenant] WHERE ID != @ID AND Name = @Name)
	begin
		UPDATE sec.[Tenant]
		SET Name = @Name,
			Settings = @Settings,
			ParentTenantID = @ParentTenantID,
			LastModifiedTime = GETDATE()
		WHERE ID = @ID
	end
END