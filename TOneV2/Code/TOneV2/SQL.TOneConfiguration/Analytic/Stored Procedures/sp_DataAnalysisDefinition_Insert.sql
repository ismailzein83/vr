--Insert
CREATE Procedure [Analytic].[sp_DataAnalysisDefinition_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@DevProjectId uniqueidentifier,
	@Settings NVARCHAR(Max)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM Analytic.DataAnalysisDefinition WHERE Name = @Name)
	BEGIN
		INSERT INTO Analytic.DataAnalysisDefinition (ID,Name,DevProjectID,Settings)
		VALUES (@ID, @Name,@DevProjectId,@Settings)
	END
END