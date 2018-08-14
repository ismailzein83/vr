
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_CDRSourceConfig_Update]
	@ID INT,
	@Name VARCHAR(100),
	@CDRSource VARCHAR(MAX),
	@SettingsTaskExecutionInfo VARCHAR(MAX),
	@IsPartnerCDRSource BIT,
	@UserID INT
AS
BEGIN
	IF NOT EXISTS(SELECT NULL FROM dbo.CDRSourceConfig WHERE CDRSourceConfigID != @ID AND Name = @Name AND UserID = @UserID)
	BEGIN
		UPDATE dbo.CDRSourceConfig
		SET Name = @Name, CDRSource = @CDRSource, IsPartnerCDRSource = @IsPartnerCDRSource, SettingsTaskExecutionInfo = @SettingsTaskExecutionInfo, UserID = @UserID
		WHERE CDRSourceConfigID = @ID
	END
END