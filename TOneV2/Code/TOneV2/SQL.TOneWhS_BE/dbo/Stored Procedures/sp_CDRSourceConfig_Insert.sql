
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_CDRSourceConfig_Insert]
	@Name VARCHAR(100),
	@CDRSource VARCHAR(MAX),
	@SettingsTaskExecutionInfo VARCHAR(MAX),
	@IsPartnerCDRSource BIT,
	@UserID INT,
	@ID INT OUT
AS
BEGIN
	IF NOT EXISTS(SELECT NULL FROM dbo.CDRSourceConfig WHERE Name = @Name AND UserID = @UserID)
	BEGIN
		INSERT INTO dbo.CDRSourceConfig (Name, CDRSource, SettingsTaskExecutionInfo, IsPartnerCDRSource, UserID)
		VALUES (@Name, @CDRSource, @SettingsTaskExecutionInfo, @IsPartnerCDRSource, @UserID)
		SET @ID = SCOPE_IDENTITY()
	END
END