--GetAll
CREATE Procedure [common].[sp_MailMessageType_GetAll]
AS
BEGIN
	select	ID, Name, Settings
	from	[common].[MailMessageType] WITH(NOLOCK) 
	ORDER BY [Name]
END