CREATE PROCEDURE [TOneWhS_BE].[sp_Switch_Update]
	@ID int,
	@Name nvarchar(255),
	@Settings nvarchar(max),
	@LastModifiedBy int
AS
BEGIN
IF NOT EXISTS(select 1 from TOneWhS_BE.Switch where Name = @Name and Id!=@ID) 
BEGIN
	Update TOneWhS_BE.Switch
	Set Name = @Name, Settings = @Settings, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
	Where ID = @ID
END
END