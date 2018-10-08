
CREATE Procedure [bp].sp_ProcessSynchronisation_SetDisable
	@ID uniqueidentifier,
	@LastModifiedBy int
AS
BEGIN
	update [bp].[ProcessSynchronisation]
	set IsEnabled = 0,
		LastModifiedBy = @LastModifiedBy,
		LastModifiedTime = getdate()
	where ID = @ID
END