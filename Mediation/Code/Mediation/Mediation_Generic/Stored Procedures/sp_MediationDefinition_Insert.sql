CREATE PROCEDURE [Mediation_Generic].[sp_MediationDefinition_Insert]	
	@ID uniqueidentifier, 
	@Name nvarchar(255),
	@Details VARCHAR(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [Mediation_Generic].[MediationDefinition] WHERE Name = @Name)
	BEGIN
		INSERT INTO [Mediation_Generic].[MediationDefinition](ID,Name,Details,CreatedTime)
		VALUES (@ID,@Name,@Details,GETDATE())

	END
END