CREATE PROCEDURE [sec].[sp_Module_Insert] 
	@Id uniqueidentifier,
	@Name Nvarchar(255),
	@ParentId uniqueidentifier,
	@DefaultViewId uniqueidentifier,
	@AllowDynamic bit,
	@Settings nvarchar(max)
AS
BEGIN
IF NOT EXISTS(select null from sec.[Module] where Name = @Name and ParentId = @ParentId)
	BEGIN
		Insert into sec.[Module] (ID,[Name],ParentId,DefaultViewId,AllowDynamic,Settings)
		values(@Id,@Name,@ParentId,@DefaultViewId,@AllowDynamic,@Settings)
	END
END