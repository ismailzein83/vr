--GetAll
Create Procedure [common].[sp_MailMessageType_GetAll]
AS
BEGIN
	select	ID, Name, Settings
	from	[common].[MailMessageType]
END