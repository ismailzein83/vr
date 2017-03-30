-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE[VRNotification].[sp_VRAlertLevel_GetAll]
AS
BEGIN
	SELECT ID, Name, Settings, BusinessEntityDefinitionID
	FROM [VRNotification].VRAlertLevel  with(nolock)
END