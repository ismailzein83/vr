
CREATE PROCEDURE [bp].[sp_ProcessSynchronisation_Insert]
	@ProcessSynchronisationId uniqueidentifier,
	@Name NVARCHAR(255),
	@IsEnabled bit,
	@Settings NVARCHAR(MAX),
	@CreatedBy int
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [bp].[ProcessSynchronisation] WHERE Name = @Name)
	BEGIN
		INSERT INTO [bp].[ProcessSynchronisation] (ID, Name,IsEnabled, Settings, CreatedBy, LastModifiedBy)
		VALUES (@ProcessSynchronisationId, @Name, @IsEnabled, @Settings, @CreatedBy, @CreatedBy)
	END
END