CREATE PROCEDURE [sec].[sp_Module_Update] 
	@ID uniqueidentifier,
	@Name Nvarchar(255),
	@ParentId uniqueidentifier,	
	@DefaultViewId uniqueidentifier,
	@AllowDynamic bit,
	@Settings nvarchar(max)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM sec.[Module] WHERE ID != @ID AND Name = @Name and ParentId = @ParentId)
	begin
		UPDATE sec.[Module]
		SET Name = @Name,
			ParentId = @ParentId,
			[DefaultViewId] = @DefaultViewId,
			[AllowDynamic] = @AllowDynamic,
			[Settings] = @Settings
		WHERE ID = @ID
	end
END