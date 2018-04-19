CREATE Procedure [common].[sp_SMSMessageTemplate_Update]
	@ID uniqueidentifier,
	@Name nvarchar(255),
	@MessageTypeID uniqueidentifier,
	@Settings nvarchar(MAX),
	@LastModifiedBy int
	
AS
BEGIN
	IF NOT EXISTS(SELECT 1 from [common].[SMSMessageTemplate] where ID != @ID AND Name = @Name)
	BEGIN
		update [common].[SMSMessageTemplate]
		set Name = @Name, Settings = @Settings, SMSMessageTypeId = @MessageTypeID, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
		where ID = @ID
	END
END