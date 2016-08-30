CREATE PROCEDURE [TOneWhS_BE].[sp_ZoneServiceConfig_Update]
	@ID int ,
	@Symbol nvarchar(50),
	@Settings nvarchar(max)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM TOneWhS_BE.[ZoneServiceConfig] WHERE (Symbol = @Symbol and ID!=@ID))
	BEGIN
		Update TOneWhS_BE.ZoneServiceConfig
		Set Symbol = @Symbol,
		 Settings=@Settings
		Where ID = @ID
	END
END