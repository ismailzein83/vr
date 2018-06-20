CREATE PROCEDURE [sec].[sp_RegisteredApplication_Insert]
	@ApplicationId uniqueidentifier,
	@Name varchar(50), 	
	@URL varchar(50)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM [sec].[RegisteredApplication] WHERE @ApplicationId = Id)
	BEGIN
		INSERT INTO [sec].[RegisteredApplication](Id, Name, URL)
		VALUES (@ApplicationId, @Name, @URL)
	END
END