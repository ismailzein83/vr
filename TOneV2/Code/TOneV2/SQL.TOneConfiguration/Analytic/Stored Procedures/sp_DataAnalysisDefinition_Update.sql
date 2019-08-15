--Update
CREATE Procedure [Analytic].[sp_DataAnalysisDefinition_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@DevProjectId uniqueidentifier,
	@Settings NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM Analytic.DataAnalysisDefinition WHERE ID != @ID and Name = @Name)
	BEGIN
		update Analytic.DataAnalysisDefinition
		set  Name = @Name,DevProjectID=@DevProjectId, Settings = @Settings, LastModifiedTime = getdate()
		where  ID = @ID
	END
END