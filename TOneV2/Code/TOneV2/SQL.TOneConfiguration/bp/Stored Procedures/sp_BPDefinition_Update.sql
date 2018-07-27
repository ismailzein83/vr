
CREATE PROCEDURE  [bp].[sp_BPDefinition_Update]
	@BPDefinitionId uniqueidentifier,
	@Title nvarchar(255),
	@VRWorkflowId uniqueidentifier,
	@Config nvarchar(MAX)
AS
BEGIN
  	IF NOT EXISTS(Select Title from [bp].[BPDefinition] WITH(NOLOCK) where Title = @Title And ID != @BPDefinitionId)
	BEGIN
		Update [bp].[BPDefinition] 
		set	Title = @Title,	VRWorkflowId = @VRWorkflowId, Config = @Config		
		where ID = @BPDefinitionId
	END
END