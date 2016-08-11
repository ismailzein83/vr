--Update
CREATE Procedure [common].[sp_MailMessageType_Update]
	@ID uniqueidentifier,
	@Name nvarchar(255),
	@Settings nvarchar(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 from [common].[MailMessageType] where ID != @ID AND Name = @Name)
	BEGIN
		update [common].[MailMessageType]
		set Name = @Name, Settings = @Settings
		where ID = @ID
	END
END