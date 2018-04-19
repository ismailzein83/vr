CREATE PROCEDURE [common].[sp_SMSMessageTemplate_GetAll]
AS
BEGIN
	select	ID, Name, SMSMessageTypeId, Settings, CreatedBy, CreatedTime, LastModifiedBy, LastModifiedTime
	from	[common].[SMSMessageTemplate] WITH(NOLOCK) 
	ORDER BY [Name]
END