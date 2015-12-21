
CREATE PROCEDURE [TOneWhS_BE].[sp_ZoneServiceConfig_Update]
	@ServiceFlag smallint ,
	@Name nvarchar(255),
	@Settings nvarchar(max)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM TOneWhS_BE.[ZoneServiceConfig] WHERE (Name = @Name and ServiceFlag!=@ServiceFlag))
	BEGIN
		Update TOneWhS_BE.ZoneServiceConfig
		Set Name = @Name, Settings=@Settings
		Where ServiceFlag = @ServiceFlag
	END
	
END