CREATE PROCEDURE [common].[sp_VRTimeZone_Update]
	@ID int ,
	@Name nvarchar(255),
	@Settings nvarchar(max),
	@LastModifiedBy int
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM common.VRTimeZone WHERE  ID != @ID and Name = @Name)	
	BEGIN
		Update [common].[VRTimeZone]
		set Name=@Name ,Settings=@Settings, LastModifiedBy=@LastModifiedBy, LastModifiedTime = GETDATE()
		Where ID = @ID
	END
END