-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_SystemAction_GetAll]
AS
BEGIN
	SELECT ID, Name, RequiredPermissions
	FROM	[sec].[SystemAction] WITH(NOLOCK) 
	ORDER BY [Name]
END