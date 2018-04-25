CREATE PROCEDURE [runtime].[sp_RuntimeNode_Update]
	@ID uniqueidentifier,
	@RuntimeNodeConfigurationId uniqueidentifier,
	@Name nvarchar(255),
	@Setting nvarchar(max)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM runtime.RuntimeNode WHERE ID != @ID AND Name = @Name)
	BEGIN
		Update runtime.[RuntimeNode]
	Set Name = @Name,
	RuntimeNodeConfigurationId=@RuntimeNodeConfigurationId,
	Settings = @Setting
	Where ID = @ID
	END
END