-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE logging.sp_ActionAuditLKUP_GetAll	
AS
BEGIN
	SELECT [ID]
      ,[Type]
      ,[Name] FROM [logging].[ActionAuditLKUP] WITH (NOLOCK)
END