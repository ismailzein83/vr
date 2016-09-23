CREATE PROCEDURE [sec].[sp_Module_Insert] 
	@Id uniqueidentifier,
	@Name Nvarchar(255),
	@ParentId uniqueidentifier,
	@AllowDynamic bit
AS
BEGIN
IF NOT EXISTS(select null from sec.[Module] where Name = @Name and ParentId = @ParentId)
	BEGIN
		Insert into sec.[Module] (ID,[Name],ParentId,AllowDynamic)
		values(@Id,@Name,@ParentId, @AllowDynamic)
	END
END