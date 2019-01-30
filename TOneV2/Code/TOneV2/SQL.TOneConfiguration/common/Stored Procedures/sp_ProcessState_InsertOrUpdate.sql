
CREATE PROCEDURE [common].[sp_ProcessState_InsertOrUpdate]
@UniqueName varchar(255),
@Settings nvarchar(max)
AS
BEGIN
	if Not Exists (Select 1 from common.ProcessState WITH(NOLOCK) where UniqueName = @UniqueName)
		Begin
			Insert Into common.ProcessState (UniqueName, Settings)
			Values ( @UniqueName, @Settings)
		End
	else
		Begin
			Update common.ProcessState 
			set Settings = @Settings
			Where UniqueName = @UniqueName
		End
END