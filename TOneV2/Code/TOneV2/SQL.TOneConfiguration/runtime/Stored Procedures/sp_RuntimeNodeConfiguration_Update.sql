CREATE PROCEDURE [runtime].[sp_RuntimeNodeConfiguration_Update]
	@ID uniqueidentifier,
	@Name nvarchar(255)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM runtime.RuntimeNodeConfiguration WHERE ID != @ID AND Name = @Name)
	BEGIN
		Update runtime.[RuntimeNodeConfiguration]
	Set Name = @Name
	Where ID = @ID
	END
END