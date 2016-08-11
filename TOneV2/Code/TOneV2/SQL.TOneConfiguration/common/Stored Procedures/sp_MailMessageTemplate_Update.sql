create Procedure [common].[sp_MailMessageTemplate_Update]
	@ID uniqueidentifier,
	@Name nvarchar(255),
	@MessageTypeID uniqueidentifier,
	@Settings nvarchar(MAX)
	
AS
BEGIN
	IF NOT EXISTS(SELECT 1 from [common].[MailMessageTemplate] where ID != @ID AND Name = @Name)
	BEGIN
		update [common].[MailMessageTemplate]
		set Name = @Name, Settings = @Settings, MessageTypeID = @MessageTypeID
		where ID = @ID
	END
END