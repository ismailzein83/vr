CREATE Procedure [common].[sp_MailMessageTemplate_GetAll]
AS
BEGIN
	select	ID, Name, MessageTypeID, Settings, CreatedTime
	from	[common].[MailMessageTemplate] WITH(NOLOCK) 
	ORDER BY [Name]
END