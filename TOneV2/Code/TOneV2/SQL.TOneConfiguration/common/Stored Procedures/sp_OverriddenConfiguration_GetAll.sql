-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].sp_OverriddenConfiguration_GetAll
	AS
BEGIN
	SELECT ID, Name, Settings, GroupID
	FROM [Common].OverriddenConfiguration  with(nolock)
END