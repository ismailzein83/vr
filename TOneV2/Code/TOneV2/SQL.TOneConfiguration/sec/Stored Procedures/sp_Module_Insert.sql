﻿CREATE PROCEDURE [sec].[sp_Module_Insert] 
	@Id uniqueidentifier,
	@Name Nvarchar(255),
	@DevProjectId uniqueidentifier,
	@ParentId uniqueidentifier,
	@DefaultViewId uniqueidentifier,
	@AllowDynamic bit,
	@Settings nvarchar(max),	
	@RenderedAsView bit
AS
BEGIN
IF NOT EXISTS(select null from sec.[Module] where Name = @Name and ParentId = @ParentId)
	BEGIN
		Insert into sec.[Module] (ID,[Name],DevProjectId,ParentId,DefaultViewId,AllowDynamic,Settings,RenderedAsView)
		values(@Id,@Name,@DevProjectId,@ParentId,@DefaultViewId,@AllowDynamic,@Settings,@RenderedAsView)
	END
END