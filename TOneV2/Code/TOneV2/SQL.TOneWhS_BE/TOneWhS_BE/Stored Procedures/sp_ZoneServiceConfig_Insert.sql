CREATE PROCEDURE [TOneWhS_BE].[sp_ZoneServiceConfig_Insert]
	@Symbol nvarchar(50),
	@Settings nvarchar(max),
	@Id int out
	
AS

BEGIN
SET @Id =0;
IF NOT EXISTS(SELECT 1 FROM TOneWhS_BE.[ZoneServiceConfig] WHERE Symbol = @Symbol )
	BEGIN
		INSERT INTO TOneWhS_BE.ZoneServiceConfig(Symbol, Settings)
		VALUES ( @Symbol, @Settings)
		Set @Id = SCOPE_IDENTITY()
	END
END