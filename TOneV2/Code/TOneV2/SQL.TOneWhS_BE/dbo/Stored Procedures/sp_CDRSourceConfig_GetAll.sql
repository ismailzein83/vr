
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_CDRSourceConfig_GetAll]
AS
BEGIN
	SELECT CDRSourceConfigID,
		Name,
		CDRSource,
		SettingsTaskExecutionInfo,
		IsPartnerCDRSource,
		UserID
	FROM dbo.CDRSourceConfig WITH(NOLOCK) 
END