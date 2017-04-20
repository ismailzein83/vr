CREATE PROCEDURE [Mediation_Generic].[sp_MediationDefinition_Insert] 
	@Name nvarchar(255),
	@Details VARCHAR(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [Mediation_Generic].[MediationDefinition] WHERE Name = @Name)
	BEGIN
		INSERT INTO [Mediation_Generic].[MediationDefinition](Name,Details,CreatedTime)
		VALUES (@Name,@Details,GETDATE())

	END
END