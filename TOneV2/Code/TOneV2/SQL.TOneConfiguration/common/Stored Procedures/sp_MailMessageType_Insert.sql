CREATE Procedure [common].[sp_MailMessageType_Insert]
	@ID uniqueidentifier,
	@Name nvarchar(255),
	@Settings nvarchar(MAX)

AS
BEGIN
IF NOT EXISTS(select 1 from [common].[MailMessageType] where Name = @Name)
	BEGIN
		insert into [common].[MailMessageType] ([ID], [Name], [Settings])
		values(@ID, @Name, @Settings)
	END
END