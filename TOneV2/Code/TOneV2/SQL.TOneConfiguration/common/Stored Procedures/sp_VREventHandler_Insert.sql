CREATE PROCEDURE [common].sp_VREventHandler_Insert
	@VREventHandlerId uniqueidentifier, 
	@Name nvarchar(255),
	@Settings nvarchar(max),
	@BED datetime,
	@EED datetime
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM common.VREventHandler WHERE Name = @Name)
	BEGIN
		INSERT INTO common.VREventHandler(ID,Name,Settings, BED,EED)
		VALUES (@VREventHandlerId,@Name, @Settings,@BED,@EED)
	END
END