CREATE Procedure [common].[sp_MailMessageType_Insert]
	@ID uniqueidentifier,
	@Name nvarchar(255),
	@DevProjectId uniqueidentifier,
	@Settings nvarchar(MAX)

AS
BEGIN
IF NOT EXISTS(select 1 from [common].[MailMessageType] where Name = @Name)
	BEGIN
		insert into [common].[MailMessageType] ([ID], [Name],[DevProjectId], [Settings])
		values(@ID, @Name,@DevProjectId, @Settings)
	END
END