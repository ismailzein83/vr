﻿CREATE PROCEDURE [sec].[sp_Module_Update] 
	@ID uniqueidentifier,
	@Name Nvarchar(255),
	@DevProjectId uniqueidentifier,
	@ParentId uniqueidentifier,	
	@DefaultViewId uniqueidentifier,
	@AllowDynamic bit,
	@Settings nvarchar(max),
	@RenderedAsView bit
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM sec.[Module] WHERE ID != @ID AND Name = @Name and ParentId = @ParentId)
	begin
		UPDATE sec.[Module]
		SET Name = @Name,
		    DevProjectId=@DevProjectId,
			ParentId = @ParentId,
			[DefaultViewId] = @DefaultViewId,
			[AllowDynamic] = @AllowDynamic,
			[Settings] = @Settings,
			LastModifiedTime = GETDATE(),
			RenderedAsView = @RenderedAsView
		WHERE ID = @ID
	end
END