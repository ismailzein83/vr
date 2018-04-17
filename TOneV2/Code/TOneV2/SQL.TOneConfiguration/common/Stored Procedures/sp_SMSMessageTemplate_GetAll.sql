CREATE PROCEDURE [common].[sp_SMSMessageTemplate_GetAll]
AS
BEGIN
	select	ID, Name, SMSMessageTypeId, Settings
	from	[common].[SMSMessageTemplate] WITH(NOLOCK) 
	ORDER BY [Name]
END