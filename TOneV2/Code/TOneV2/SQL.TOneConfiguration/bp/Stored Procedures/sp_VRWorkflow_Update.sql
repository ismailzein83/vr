
CREATE PROCEDURE  [bp].[sp_VRWorkflow_Update]
@VRWorkflowId uniqueidentifier,
@Name nvarchar(255),
@Title nvarchar(255),
@DevProjectId uniqueidentifier,
@Settings nvarchar(MAX),
@LastModifiedBy int
AS
BEGIN
  	IF NOT EXISTS(Select Name from [bp].[VRWorkflow] WITH(NOLOCK) where Name = @Name And ID != @VRWorkflowId)
	BEGIN
		Update [bp].[VRWorkflow] set
		Name = @Name,
		Title = @Title,
		DevProjectID=@DevProjectId,
		Settings = @Settings,
		LastModifiedBy = @LastModifiedBy,
	
		LastModifiedTime = getdate()
		where ID = @VRWorkflowId
	END
END