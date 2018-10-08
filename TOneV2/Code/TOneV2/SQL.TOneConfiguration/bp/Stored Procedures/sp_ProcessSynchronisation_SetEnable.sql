
CREATE Procedure [bp].[sp_ProcessSynchronisation_SetEnable]
	@ID uniqueidentifier,
	@LastModifiedBy int
AS
BEGIN
	update [bp].[ProcessSynchronisation]
	set IsEnabled = 1,
		LastModifiedBy = @LastModifiedBy,
		LastModifiedTime = getdate()
	where ID = @ID
END