
CREATE PROCEDURE [common].[sp_ProcessState_InsertOrUpdate]
@UniqueName varchar(255),
@Settings nvarchar(max)

AS
BEGIN
	IF NOT EXISTS (Select 1 from common.ProcessState WITH(NOLOCK) where UniqueName = @UniqueName)
		BEGIN
			INSERT Into common.ProcessState (UniqueName, Settings)
			Values (@UniqueName, @Settings)
		END
	ELSE
		BEGIN
			UPDATE common.ProcessState 
			set Settings = @Settings, LastModifiedTime = getdate()
			Where UniqueName = @UniqueName
		END
END