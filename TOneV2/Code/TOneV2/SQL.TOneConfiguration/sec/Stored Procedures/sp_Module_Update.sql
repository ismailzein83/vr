CREATE PROCEDURE [sec].[sp_Module_Update] 
	@ID uniqueidentifier,
	@Name Nvarchar(255),
	@ParentId uniqueidentifier,
	@AllowDynamic bit
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM sec.[Module] WHERE ID != @ID AND Name = @Name and ParentId = @ParentId)
	begin
		UPDATE sec.[Module]
		SET Name = @Name,
			ParentId = @ParentId,
			[AllowDynamic] = @AllowDynamic
		WHERE ID = @ID
	end
END