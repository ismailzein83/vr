
CREATE PROCEDURE [common].[sp_File_SetUsedAndUpdateSettings]
	-- Add the parameters for the stored procedure here
	@Id BIGINT,
	@ConfigID uniqueidentifier,
	@Settings NVARCHAR(MAX)
AS
BEGIN
	
		UPDATE	[common].[File]
		SET		[IsTemp] = 0,
				ConfigID = @ConfigID,
				Settings = @Settings
		WHERE	ID = @Id
END