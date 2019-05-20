﻿
CREATE PROCEDURE [bp].[sp_BPDefinition_Insert]
	@BPDefinitionID uniqueidentifier,
	@Name NVARCHAR(255),
	@Title NVARCHAR(255),
	@VRWorkflowId uniqueidentifier,
	@DevProjectId uniqueidentifier,
	@Config NVARCHAR(Max)
	
AS
BEGIN
IF NOT EXISTS(Select Title from [bp].[BPDefinition] WITH(NOLOCK) where Title = @Title)
	BEGIN
		INSERT INTO [bp].[BPDefinition] (ID, Name, Title, VRWorkflowId,DevProjectID, Config)
		VALUES (@BPDefinitionID, @Name, @Title, @VRWorkflowId,@DevProjectId, @Config)
	END
END