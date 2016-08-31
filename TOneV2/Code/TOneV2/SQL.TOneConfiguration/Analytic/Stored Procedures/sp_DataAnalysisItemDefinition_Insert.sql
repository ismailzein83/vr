--Insert
Create Procedure [Analytic].[sp_DataAnalysisItemDefinition_Insert]
	@ID uniqueidentifier,
	@DataAnalysisDefinitionID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(Max)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM Analytic.DataAnalysisItemDefinition WHERE Name = @Name)
	BEGIN
		INSERT INTO Analytic.DataAnalysisItemDefinition (ID, DataAnalysisDefinitionID, Name, Settings)
		VALUES (@ID, @DataAnalysisDefinitionID, @Name, @Settings)
	END
END