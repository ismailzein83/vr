--Update
Create Procedure [Analytic].[sp_DataAnalysisItemDefinition_Update]
	@ID uniqueidentifier,
	@DataAnalysisDefinitionID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM Analytic.DataAnalysisItemDefinition WHERE ID != @ID and Name = @Name)
	BEGIN
		update Analytic.DataAnalysisItemDefinition
		set  DataAnalysisDefinitionID = @DataAnalysisDefinitionID, Name = @Name, Settings = @Settings
		where  ID = @ID
	END
END