
Create PROCEDURE [bp].[sp_BPDefinition_Insert]
	@BPDefinitionID uniqueidentifier,
	@Name NVARCHAR(255),
	@Title NVARCHAR(255),
	@VRWorkflowId uniqueidentifier,
	@Config NVARCHAR(Max)
AS
BEGIN
	INSERT INTO [bp].[BPDefinition] (ID, Name, Title, VRWorkflowId, Config)
	VALUES (@BPDefinitionID, @Name, @Title, @VRWorkflowId, @Config)
END