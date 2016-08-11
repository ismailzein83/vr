create Procedure [common].[sp_MailMessageTemplate_Insert]
	@ID uniqueidentifier,
	@Name nvarchar(255),
	@MessageTypeID uniqueidentifier,
	@Settings nvarchar(MAX)

AS
BEGIN
IF NOT EXISTS(select 1 from [common].[MailMessageTemplate] where Name = @Name)
	BEGIN
		insert into [common].[MailMessageTemplate] ([ID], [Name], [MessageTypeID], [Settings])
		values(@ID, @Name, @MessageTypeID, @Settings)
	END
END