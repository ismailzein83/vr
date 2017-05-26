create PROCEDURE [common].sp_VREventHandler_Update
	@VREventHandlerId uniqueidentifier, 
	@Name nvarchar(255),
	@Settings nvarchar(max),
	@BED datetime,
	@EED datetime
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM common.VREventHandler WHERE ID != @VREventHandlerId AND Name = @Name)
	BEGIN
	Update common.VREventHandler
	Set Name = @Name, Settings = @Settings,BED = @BED,EED = @EED
	Where ID = @VREventHandlerId
	END
END