
CREATE PROCEDURE [TOneWhS_BE].[sp_ZoneServiceConfig_Insert]
	@ServiceFlag smallint ,
	@Name nvarchar(255),
	@Settings nvarchar(max)
AS

BEGIN

IF NOT EXISTS(SELECT 1 FROM TOneWhS_BE.[ZoneServiceConfig] WHERE Name = @Name or ServiceFlag=@ServiceFlag)
	BEGIN
		INSERT INTO TOneWhS_BE.ZoneServiceConfig(ServiceFlag, Name, Settings)
		VALUES (@ServiceFlag, @Name, @Settings)
	END
END