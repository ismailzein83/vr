
CREATE PROCEDURE [bp].[sp_ProcessSynchronisation_Update]
	@ProcessSynchronisationId uniqueidentifier,
	@Name NVARCHAR(255),
	@IsEnabled bit,
	@Settings NVARCHAR(MAX),
	@LastModifiedBy int
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [bp].[ProcessSynchronisation] WHERE Name = @Name and ID != @ProcessSynchronisationId )
	BEGIN
		Update [bp].[ProcessSynchronisation]
		SET Name = @Name,
		IsEnabled = @IsEnabled,
		Settings = @Settings,
		LastModifiedBy = @LastModifiedBy,
		LastModifiedTime = getdate()
		WHERE ID = @ProcessSynchronisationId
	END
END