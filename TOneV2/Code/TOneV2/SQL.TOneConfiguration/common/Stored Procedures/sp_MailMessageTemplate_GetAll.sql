Create Procedure [common].[sp_MailMessageTemplate_GetAll]
AS
BEGIN
	select	ID, Name, MessageTypeID, Settings
	from	[common].[MailMessageTemplate]
END