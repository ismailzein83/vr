
CREATE PROCEDURE [common].[sp_File_SetUsedAndUpdateSettingsByFileUniqueId]
	-- Add the parameters for the stored procedure here
	@FileUniqueId uniqueidentifier,
	@ConfigID uniqueidentifier,
	@Settings NVARCHAR(MAX)
AS
BEGIN
	
		UPDATE	[common].[File]
		SET		[IsTemp] = 0,
				ConfigID = @ConfigID,
				Settings = @Settings
		WHERE	FileUniqueId = @FileUniqueId
END